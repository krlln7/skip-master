using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    async static Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<UniversityService>();
        builder.Services.AddSingleton<IProgressReporter, LoggerProgressReporter>();

        var strategyName = builder.Configuration["Student:Strategy"] ?? "NoSkipStrategy";
        var strategyType = TypesResolver.ResolveByName<IStrategy>(strategyName);
        builder.Services.AddSingleton(typeof(IStrategy), strategyType);
        builder.Services.AddSingleton<University>();

        var host = builder.Build();
        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            ILogger logger = host.Services
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Program");

            logger.LogError(ex, "Unhandled exception occurred during job execution.");
        }
    }
}