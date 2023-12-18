
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
    private static ILogger<Program> _logger;
    private static IConfiguration _config;
    
    static async Task<int> Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
        
        _config = builder.Build();
        
        var services = new ServiceCollection();
        ConfigureServices(services);
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        SetupAuthentication();
        
        var importer = new EndpointImporter(_logger, _username, _password);
        // await importer.Start();
        
        await TestFrost(_config["FrostBaseUrl"]);
        
        Process.GetCurrentProcess().WaitForExit();
        
        return 1;
    }
    
    static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Information);
    }

    static void SetupAuthentication()
    {
        var authentication = _config.GetSection("Authentication");
        
        if (!authentication.Exists())
            throw new Exception("Authentication section not found in appsettings.json");
        
        _username = authentication["Username"];
        _password = authentication["Password"];
        
        _logger.LogInformation($"{_username}, {_password}");
        
        if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
        {
            throw new Exception("Username or password is empty");
        }
    }
    
    static async Task TestFrost(string baseUrl)
    {
        var api = new FrostApi.FrostApi(baseUrl);
        var things = api.Things;
        var thingsResponse = await things.GetAllThings();
        // var response = await things.PostThing(new Tree()
        // {
        //     Name = "BaumTest",
        //     Description = "TreeSense tree test"
        // });
        var response = await things.UpdateThing(new Tree()
        {
            Id = thingsResponse.Value.Last().Id,
            Name = "BaumTest editiert 2",
        });
        // Console.WriteLine(thingsResponse.Value[1].Name);
        Console.WriteLine(response.StatusCode);
    }

}