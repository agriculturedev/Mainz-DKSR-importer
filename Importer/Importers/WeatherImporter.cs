using DKSRDomain;
using FrostApi.Models.Thing;
using Importer.Constants;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class WeatherImporter : Importer
{
    private Timer _importerTimer;

    public WeatherImporter(ILogger logger) : base(logger, "Weather", "weather")
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 60); // every hour
    }

    protected override async void Import(object? _)
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetDksrData();
            foreach (var weather in data.SensorData)
                try
                {
                    Thing thing;
                    var frostThing = await GetFrostThingData(weather.SID);
                    if (frostThing.Value.Count == 0)
                    {
                        thing = Mappers.MapDksrResponse(weather, DataType);
                        await CreateNewThing(thing);
                        frostThing = await GetFrostThingData(weather.SID);
                    }

                    if (frostThing.Value.Count < 1)
                        throw new Exception($"Creating new thing with id {weather.SID} seems to have failed...");

                    thing = Mappers.MapDksrResponse(weather, DataType);
                    thing.Id = frostThing.Value.First().Id;
                    await Update(thing);
                }
                catch (Exception e)
                {
                    Logger.LogError($"{DateTime.Now} - {e}");
                }
        }
        catch (Exception e)
        {
            Logger.LogError($"{DateTime.Now} - {e}");
        }
    }


    private async Task<WeatherResponse> GetDksrData()
    {
        try
        {
            var response =
                await Client.GetAsync(
                    Endpoints.GetAuthenticatedDKSREndpointUrl(Username, Password, Endpoints.WeatherEndpoint));
            var result = await response.Content.ReadAsAsync<WeatherResponse>();
            return result;
        }
        catch (Exception e)
        {
            Logger.LogError("Getting data from DKSR failed, returning empty response");
            throw new Exception($"{DateTime.Now} - {e}");
        }
    }
}