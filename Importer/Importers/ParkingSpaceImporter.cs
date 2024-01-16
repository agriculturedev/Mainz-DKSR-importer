using DKSRDomain;
using FrostApi.Models.Thing;
using Importer.Constants;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class ParkingSpaceImporter : Importer
{
    private Timer _importerTimer;

    public ParkingSpaceImporter(ILogger logger) : base(logger, "ParkingSpace", "Occupancy")
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 2); // every 2 minutes
    }

    protected override async void Import(object? _) // object? required for running in a timer
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetDksrData();
            foreach (var parkingSpace in data.SensorData)
                try
                {
                    Thing thing;
                    var frostThing = await GetFrostThingData(parkingSpace.Sid);
                    if (frostThing.Value.Count == 0)
                    {
                        thing = Mappers.MapDksrResponse(parkingSpace, DataType);
                        await CreateNewThing(thing);
                        frostThing = await GetFrostThingData(parkingSpace.Sid);
                    }

                    if (frostThing.Value.Count < 1)
                        throw new Exception($"Creating new thing with id {parkingSpace.Sid} seems to have failed...");

                    thing = Mappers.MapDksrResponse(parkingSpace, DataType);
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

    private async Task<ParkingSpaceResponse> GetDksrData()
    {
        try
        {
            var response =
                await Client.GetAsync(
                    Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.ParkingSpaceEndpoint));
            var result = await response.Content.ReadAsAsync<ParkingSpaceResponse>();
            return result;
        }
        catch (Exception e)
        {
            Logger.LogError("Getting data from DKSR failed, returning empty response");
            throw new Exception($"{DateTime.Now} - {e}");
        }
    }
}