using Microsoft.Extensions.Hosting;

class UniversityService : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IProgressReporter _logger;
    private readonly University _uni;

    public UniversityService(IHostApplicationLifetime lifetime, IProgressReporter logger, University uni)
    {
        _lifetime = lifetime;
        _logger = logger;
        _uni = uni;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            bool alive = _uni.RunOneDay();
            _logger.Report(_uni.Day, _uni.Student);

            if (!alive)
            {
                _lifetime.StopApplication();
                return;
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}