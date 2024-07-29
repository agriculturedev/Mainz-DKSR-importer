using DKSRDomain;
using Importer.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class WeatherImporter : Importer
{
    private Timer _importerTimer;

    public WeatherImporter(ILogger logger, DataSource dataSource) : base(logger, "Weather", "weather", dataSource)
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 60); // every hour
    }

    protected override async void Import(object? _)
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetDksrData<WeatherSensorData>();
            foreach (var weather in data)
            {
                try
                {
                    var thing = Mappers.MapDksrResponse(weather, DataType);
                    UpdateThing(thing);
                }
                catch (Exception e)
                {
                    Logger.LogError($"{DateTime.Now} - {e}");
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"{DateTime.Now} - {e}");
        }
    }
}