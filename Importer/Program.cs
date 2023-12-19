using System.Diagnostics;
using FrostApi.ThingImplementations;
using Importer.Importers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Importer;

internal class Program
{
    private static string _username = "";
    private static string _password = "";
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

        // var importer = new EndpointImporter(_logger, _config);
        // await importer.Start();

        var treeImporter = new TreeImporter(_logger, _config);
        // var parkingLotImporter = new ParkingLotImporter(_logger, _config);

        // await TestFrost(_config["FrostBaseUrl"]);

        Process.GetCurrentProcess().WaitForExit();

        return 1;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Debug);
    }

    private static async Task TestFrost(string baseUrl)
    {
        var api = new FrostApi.FrostApi(baseUrl);
        var things = api.Things;
        var thingsResponse = await things.GetAllThings();
        // var response = await things.PostThing(new Tree()
        // {
        //     Name = "BaumTest",
        //     Description = "TreeSense tree test"
        // });
        var response = await things.UpdateThing(new Tree
        {
            Id = thingsResponse.Value.Last().Id,
            Name = "BaumTest editiert 2"
        });
        // Console.WriteLine(thingsResponse.Value[1].Name);
        Console.WriteLine(response.StatusCode);
    }
}