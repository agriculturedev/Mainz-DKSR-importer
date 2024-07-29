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

        var importers = new List<Importers.Importer>();

        foreach (var source in ConfigurationManager.Sources.Sources)
        {
            _logger.LogInformation($"Importing {source.ImporterType}...");
            switch (source.ImporterType.ToLower())
            {
                case "tree":
                    var treeImporter = new TreeImporter(_logger, source);
                    importers.Add(treeImporter);
                    break;
                case "parkingspace":
                    var parkingSpaceImporter = new ParkingSpaceImporter(_logger, source);
                    importers.Add(parkingSpaceImporter);
                    break;
                case "weather":
                    var weatherImporter = new WeatherImporter(_logger, source);
                    importers.Add(weatherImporter);
                    break;
                default:
                    _logger.LogWarning($"Unknown source: {source.ImporterType}");
                    break;
            }
        }


        Console.WriteLine($"active importers: {importers.Count()}");

        Process.GetCurrentProcess().WaitForExit();
    }
    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information);
    }
}