using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class UniversityService : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IProgressReporter _reporter;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly IStrategy _strategy;
    private readonly ILogger<UniversityService> _log;

    public UniversityService(
        IHostApplicationLifetime lifetime,
        IProgressReporter reporter,
        IServiceScopeFactory scopeFactory,
        IConfiguration config,
        IStrategy strategy,
        ILogger<UniversityService> log)
    {
        _lifetime = lifetime;
        _reporter = reporter;
        _scopeFactory = scopeFactory;
        _config = config;
        _strategy = strategy;
        _log = log;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mode = _config["Simulation:Mode"] ?? "New";
        var semesterDays = int.Parse(_config["Simulation:SemesterDays"] ?? "100");

        Guid sessionId;
        University uni;

        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<SimulationRepository>();

            if (mode == "Continue" && await repo.FindUnfinishedAsync(stoppingToken) is Guid existing)
            {
                sessionId = existing;
                uni = await repo.LoadAsync(sessionId, ResolveStrategyByName, stoppingToken);
                _log.LogInformation("Resuming session {SessionId} at day {Day}", sessionId, uni.Day);
            }
            else
            {
                var lessons = Enum.GetValues<Subject>()
                    .Select(s => new Lesson(s))
                    .ToArray();

                sessionId = await repo.CreateSessionAsync(_strategy, lessons, semesterDays, stoppingToken);
                uni = await repo.LoadAsync(sessionId, ResolveStrategyByName, stoppingToken);
                _log.LogInformation("Started new session {SessionId} ({Days} days)", sessionId, semesterDays);
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            bool alive = uni.RunOneDay();
            _reporter.Report(uni.Day, uni.Student);

            await using (var scope = _scopeFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<SimulationRepository>();
                try
                {
                    await repo.SaveDayAsync(sessionId, uni, stoppingToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Failed to persist day {Day} of session {SessionId}", uni.Day, sessionId);
                    _lifetime.StopApplication();
                    return;
                }
            }

            if (!alive)
            {
                _lifetime.StopApplication();
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private static IStrategy ResolveStrategyByName(string name)
    {
        var type = TypesResolver.ResolveByName<IStrategy>(name);
        return (IStrategy)Activator.CreateInstance(type)!;
    }
}