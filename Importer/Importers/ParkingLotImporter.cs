using DKSRDomain;
using FrostApi.Models.Thing;
using Importer.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class ParkingLotImporter : Importer
{
    private Timer _importerTimer;

    public ParkingLotImporter(ILogger logger, IConfiguration config) : base(logger, config, "ParkingLot", "Occupancy")
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000); // every minute
    }
    
    protected override async void Import(object? _) // object? required for running in a timer
    {
        try
        {
            Logger.LogInformation($"Updating {DataType} Data...");
            var data = await GetDksrData();
            foreach (var dksrParkingLot in data.SensorData)
                try
                {
                    Thing thing;
                    var frostThing = await GetFrostThingData(dksrParkingLot.Sid);
                    if (frostThing.Value.Count == 0)
                    {
                        thing = Mappers.MapDksrResponse(dksrParkingLot, DataType);
                        await CreateNewThing(thing);
                        frostThing = await GetFrostThingData(dksrParkingLot.Sid);
                    }
                    
                    if (frostThing.Value.Count < 1)
                        throw new Exception($"Creating new thing with id {dksrParkingLot.Sid} seems to have failed...");

                    thing = Mappers.MapDksrResponse(dksrParkingLot, DataType);
                    thing.Id = frostThing.Value.First().Id;
                    await Update(thing);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString());
        }
    }

    private async Task<ParkingLotResponse> GetDksrData()
    {
        var response =
            await Client.GetAsync(
                Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.ParkingLotEndpoint));
        var result = await response.Content.ReadAsAsync<ParkingLotResponse>();
        return result;
    }
}