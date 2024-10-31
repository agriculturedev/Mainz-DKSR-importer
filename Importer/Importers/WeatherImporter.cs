
using DKSRDomain;
using Importer.Configuration;
using Importer.Constants;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class WeatherImporter : Importer
{
    private Timer _importerTimer;

    public WeatherImporter(ILogger logger, DataSource dataSource) : base(logger, "hof_owm_connector", dataSource)
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 60); // every hour
    }

    protected override async void Import(object? _)
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetData();
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

    private async Task<List<WeatherSensorData>> GetData()
    {
        try
        {
            var url = Endpoints.GetAuthenticatedEndpointUrl(Username, Password, SourceUrl);
            var response = await Client.GetAsync(url);
            var result = await response.Content.ReadAsAsync<WeatherSensorDataWrapper>();
            return result.SensorData.ToList();
        }
        catch (Exception e)
        {
            Logger.LogError("Getting data failed, returning empty response");
            throw new Exception($"{DateTime.Now} - {e}");
        }
    }
}