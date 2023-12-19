using System.Diagnostics;
using Importer.Importers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Importer;

internal class Program
{
    private static ILogger<Program> _logger;
    private static IConfiguration _config;

    private static async Task<int> Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false);

        _config = builder.Build();

        var services = new ServiceCollection();
        ConfigureServices(services);
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var treeImporter = new TreeImporter(_logger, _config);
        var parkingLotImporter = new ParkingLotImporter(_logger, _config);

        Process.GetCurrentProcess().WaitForExit();

        return 1;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information);
    }
}