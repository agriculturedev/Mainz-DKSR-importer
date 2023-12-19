using DKSRDomain;
using FrostApi.ThingImplementations;
using Importer.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class TreeImporter : Importer
{
    private Timer _importerTimer;

    public TreeImporter(ILogger logger, IConfiguration config) : base(logger, config, "Tree", "HealthState")
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 60); // every 60 minutes
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
            foreach (var dksrTree in data.SensorData)
                try
                {
                    Tree tree;
                    var frostTree = await GetFrostThingData(int.Parse(dksrTree.Id));
                    if (frostTree.Value.Count == 0)
                    {
                        tree = Mappers.MapDksrResponse(dksrTree);
                        await CreateNewThing(tree);
                        frostTree = await GetFrostThingData(int.Parse(dksrTree.Id));
                    }

                    tree = Mappers.MapDksrResponse(dksrTree);
                    await Update(tree, frostTree);
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


    private async Task<TreesenseResponse> GetDksrData()
    {
        var response =
            await Client.GetAsync(
                Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.TreesenseEndpoint));
        var result = await response.Content.ReadAsAsync<TreesenseResponse>();
        return result;
    }
}