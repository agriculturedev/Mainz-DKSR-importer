using System.Diagnostics;
using Importer.Importers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Importer;

internal class Program
{
    private static ILogger<Program>? _logger;

    private static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // var treeImporter = new TreeImporter(_logger);
        // var parkingSpaceImporter = new ParkingSpaceImporter(_logger);
        var weatherImporter = new WeatherImporter(_logger);

        Process.GetCurrentProcess().WaitForExit();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information);
    }
}