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
                    Thing parkingLot;
                    var frostParkingLot = await GetFrostThingData(dksrParkingLot.ParkingSpaceId);
                    if (frostParkingLot.Value.Count == 0)
                    {
                        parkingLot = Mappers.MapDksrResponse(dksrParkingLot, DataType);
                        await CreateNewThing(parkingLot);
                        frostParkingLot = await GetFrostThingData(dksrParkingLot.ParkingSpaceId);
                    }

                    parkingLot = Mappers.MapDksrResponse(dksrParkingLot, DataType);
                    parkingLot.Id = frostParkingLot.Value.First().Id;
                    await Update(parkingLot);
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