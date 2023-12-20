using DKSRDomain;
using FrostApi.Models.Thing;
using Importer.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class TreeImporter : Importer
{
    private Timer _importerTimer;

    public TreeImporter(ILogger logger, IConfiguration config) : base(logger, config, "Tree", "HealthState")
    {
        _importerTimer = new Timer(Import, null, 0, 60 * 1000); // every minute
    }

    protected override async void Import(object? _)
    {
        try
        {
            Logger.LogInformation($"Updating {DataType} Data...");
            var data = await GetDksrData();
            foreach (var dksrTree in data.SensorData)
            {
                try
                {
                    Thing thing;
                    var frostThing = await GetFrostThingData(dksrTree.Sid);
                    if (frostThing.Value.Count == 0)
                    {
                        thing = Mappers.MapDksrResponse(dksrTree, DataType);
                        await CreateNewThing(thing);
                        frostThing = await GetFrostThingData(dksrTree.Sid);
                    }

                    if (frostThing.Value.Count < 1)
                        throw new Exception($"Creating new thing with id {dksrTree.Sid} seems to have failed...");

                    thing = Mappers.MapDksrResponse(dksrTree, DataType);
                    thing.Id = frostThing.Value.First().Id;
                    await Update(thing);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
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