using DKSRDomain;
using Importer.Configuration;
using Importer.Constants;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class ParkingSpaceImporter : Importer
{
    private Timer _importerTimer;

    public ParkingSpaceImporter(ILogger logger, DataSource dataSource) : base(logger, "ParkingSpace", "Occupancy", dataSource)
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 2); // every 2 minutes
    }

    protected override async void Import(object? _) // object? required for running in a timer
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetData<ParkingSpaceSensorData>();
            foreach (var parkingSpace in data)
            {
                try
                {
                    var thing = Mappers.MapDksrResponse(parkingSpace, DataType);
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