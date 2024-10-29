using DKSRDomain;
using Importer.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class TreeImporter : Importer
{
    private Timer _importerTimer;

    public TreeImporter(ILogger logger, DataSource dataSource) : base(logger, "Tree", "HealthState", dataSource)
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 60); // every hour
    }

    protected override async void Import(object? _)
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetDksrData<TreeSenseSensorData>();
            foreach (var dksrTree in data)
            {
                try
                {
                    var thing = Mappers.MapDksrResponse(dksrTree, DataType);
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