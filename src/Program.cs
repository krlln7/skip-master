using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

class Program
{
    async static Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<UniversityService>();
        builder.Services.AddSingleton<IProgressReporter, LoggerProgressReporter>();
        builder.Services.AddDbContext<UniversityDbContext>(o =>
            o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
        builder.Services.AddScoped<SimulationRepository>();
        builder.Services.AddSingleton<IStrategy>(_ =>
        {
            var name = builder.Configuration["Student:Strategy"] ?? "NoSkipStrategy";
            var type = TypesResolver.ResolveByName<IStrategy>(name);
            return (IStrategy)Activator.CreateInstance(type)!;
        });

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