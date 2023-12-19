using DKSRDomain;
using FrostApi.ThingImplementations;
using Importer.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class ParkingLotImporter : Importer
{
    private Timer _importerTimer;

    public ParkingLotImporter(ILogger logger, IConfiguration config) : base(logger, config, "ParkingLot", "Occupancy")
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 2); // every 2 minutes
    }

    private async void Import(object? _)
    {
        await Import();
    }

    private async Task Import()
    {
        try
        {
            Logger.LogInformation($"Updating {DataType} Data...");
            var data = await GetDksrData();
            foreach (var dksrParkingLot in data.SensorData)
                try
                {
                    ParkingLot parkingLot;
                    var frostParkingLot = await GetFrostThingData(dksrParkingLot.ParkingSpaceId);
                    if (frostParkingLot.Value.Count == 0)
                    {
                        parkingLot = Mappers.MapDksrResponse(dksrParkingLot);
                        await CreateNewThing(parkingLot);
                        frostParkingLot = await GetFrostThingData(dksrParkingLot.ParkingSpaceId);
                    }

                    parkingLot = Mappers.MapDksrResponse(dksrParkingLot);
                    await Update(parkingLot, frostParkingLot);
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