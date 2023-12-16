
using System.Diagnostics;
using FrostApi.ResponseModels;
using Importer.Importers;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Importer;

class Program
{
    private static string _username = "";
    private static string _password = "";
    private readonly ILogger<Program> _logger;

    static async Task<int> Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        
        IConfiguration config = builder.Build();
        
        var services = new ServiceCollection();
        ConfigureServices(services);
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        var _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        var authentication = config.GetSection("Authentication");
        
        if (!authentication.Exists())
            throw new Exception("Authentication section not found in appsettings.json");
        
        _username = authentication["Username"];
        _password = authentication["Password"];
        
        _logger.LogInformation($"{_username}, {_password}");
        
        if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
        {
            throw new Exception("Username or password is empty");
        }
        
        var importer = new EndpointImporter(_logger, _username, _password);
        // await importer.Start();
        
        await TestFrost(config["FrostBaseUrl"]);
        
        Process.GetCurrentProcess().WaitForExit();
        
        return 1;
    }
    
    static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information);
    }
    
    static async Task TestFrost(string baseUrl)
    {
        var api = new FrostApi.FrostApi(baseUrl);
        var things = api.Things;
        // var thing = await things.GetAllThings();
        await things.PostThing(new Thing()
        {
            Name = "BaumTest",
            Description = "TreeSense tree test"
        });
        // Console.WriteLine(thing.Value[1].Name);
    }

}