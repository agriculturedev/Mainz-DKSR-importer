using System.Net.WebSockets;
using System.Text;
using Importer.Constants;
using Importer.Importers;
using Microsoft.Extensions.Configuration;

namespace Importer;

class Program
{
    private static string _username = "";
    private static string _password = "";
    private static string _token = "";
    private static List<ClientWebSocket> _webSockets = new List<ClientWebSocket>();


    static async Task<int> Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();

        var authentication = config.GetSection("Authentication");

        if (!authentication.Exists())
            throw new Exception("Authentication section not found in appsettings.json");

        _username = authentication["Username"];
        _password = authentication["Password"];

        Console.WriteLine($"{_username}, {_password}");

        if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
        {
            throw new Exception("Username or password is empty");
        }

        var importer = new EndpointImporter(_username, _password);
        await importer.Start();
        
        return 1;
    }

}