using System.Reflection;
using Importer.Configuration;
using Microsoft.Extensions.Configuration;

namespace Importer;

public static class ConfigurationManager
{
    private static string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    public static IConfiguration AppSetting { get; }
    public static DataSources Sources { get; }
    
    static ConfigurationManager()
    {
        AppSetting = new ConfigurationBuilder()
            .SetBasePath(currentPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()// your path here
            .Build();

        Sources = AppSetting.Get<DataSources>();
    }
}