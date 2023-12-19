using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Location;
using FrostApi.Models.Observation;
using FrostApi.Models.ObservedProperty;
using FrostApi.Models.Sensor;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.ObservedProperty;
using FrostApi.ResponseModels.Sensor;
using FrostApi.ResponseModels.Thing;
using FrostApi.ThingImplementations;
using Importer.Constants;
using Importer.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class TreeImporter : Importer
{
    private Timer _importerTimer;

    public TreeImporter(ILogger logger, IConfiguration config) : base(logger, config)
    {
        Logger.LogInformation("Starting TreeSense Sensor Data Collection");
        _importerTimer = new Timer(Import, null, 0, 60 * 1000);
    }

    private async void Import(object? _)
    {
        await Import();
    }

    private async Task Import()
    {
        try
        {
            Logger.LogInformation("Updating Treesense Data...");
            var data = await GetDksrTreeData();
            foreach (var dksrTree in data.SensorData)
                try
                {
                    var frostTree = await GetFrostTreeData(int.Parse(dksrTree.Id));
                    if (frostTree.Value.Count == 0)
                    {
                        await CreateNewTree(dksrTree);
                        frostTree = await GetFrostTreeData(int.Parse(dksrTree.Id));
                    }

                    await UpdateTree(dksrTree, frostTree);
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

    private Task CreateNewTree(TreeSenseSensorData dksrTree)
    {
        var tree = TreeMapper.MapDksrResponseToTree(dksrTree);

        Logger.LogDebug($"Tree {tree.Name} with Id {tree.Properties.Id} not found in Frost, creating new tree");
        var postResponse = FrostApi.Things.PostThing(tree).Result;
        if (postResponse.IsSuccessStatusCode)
            Logger.LogDebug($"Tree {tree.Name} with Id {tree.Properties.Id} created successfully");
        else
            Logger.LogError($"Tree {tree.Name} with Id {tree.Properties.Id} could not be created");
        return Task.CompletedTask;
    }

    private async Task UpdateTree(TreeSenseSensorData dksrTree, GetThingsResponse frostTree)
    {
        var tree = TreeMapper.MapDksrResponseToTree(dksrTree);

        Logger.LogDebug($"Tree {dksrTree.Name} with Id {dksrTree.Id} found in Frost, updating tree");

        tree.Id = frostTree.Value.First().Id;
        var response = await FrostApi.Things.UpdateThing(tree);

        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug($"Tree {tree.Name} with Id {tree.Properties.Id} updated successfully");
            await UpdateDataStream(dksrTree, tree);
        }
        else
        {
            Logger.LogError($"Tree {tree.Name} with Id {tree.Properties.Id} could not be updated");
        }
    }

    private async Task UpdateDataStream(TreeSenseSensorData dksrTree, Tree tree)
    {
        var dataStreams = await GetOrCreateDataStream(dksrTree, tree);

        var healthStateDataStream =
            DataStreamMapper.MapFrostResponseToDataStream(dataStreams.Value.Find(dataStream =>
                dataStream?.Name == "HealthState"));

        await CreateLocationIfNotExists(dksrTree, tree);

        // var featureOfInterestResponse = await FrostApi.FeatureOfInterest.GetAllFeaturesOfInterest($"?$filter=description eq 'Tree' &$filter= properties/id eq '{tree.Properties.Id}'");

        await AddObservation(dksrTree, healthStateDataStream);
    }

    private async Task CreateLocationIfNotExists(TreeSenseSensorData dksrTree, Tree tree)
    {
        var locations = await FrostApi.Locations.GetLocationsForThing(tree.Id);
        if (locations?.Value == null || locations.Value.Count == 0)
        {
            await CreateNewLocation(dksrTree, tree);

            locations = await FrostApi.Locations.GetLocationsForThing(tree.Id);
            if (locations?.Value == null || locations.Value.Count == 0)
            {
                Logger.LogError("unable to create new Location");
                throw new Exception("unable to create new Location");
            }
        }
    }

    private async Task CreateNewLocation(TreeSenseSensorData dksrTree, Tree tree)
    {
        var location = new ThingLocation
        {
            Name = "Location",
            Description = "Location of a tree",
            EncodingType = "application/geo+json",
            Location = new LocationProperties
            {
                Type = "Point",
                Coordinates = new List<string> { dksrTree.Lat, dksrTree.Lng }
            },
            Things = new List<Dictionary<string, string>> { new() { { "@iot.id", tree.Id.ToString() } } }
        };

        var response = await FrostApi.Locations.PostLocation(location);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Location {location.Name} created successfully");
        else
            Logger.LogError($"Location {location.Name} could not be created");
    }

    private async Task<GetDataStreamsResponse> GetOrCreateDataStream(TreeSenseSensorData dksrTree, Tree tree)
    {
        var dataStreams = await GetFrostDataStreamData(tree.Id);
        if (dataStreams?.Value == null || dataStreams.Value.Count == 0)
        {
            await CreateNewDataStream(dksrTree, tree);

            dataStreams = await GetFrostDataStreamData(tree.Id);
            if (dataStreams?.Value == null || dataStreams.Value.Count == 0)
            {
                Logger.LogError("unable to create new HealthState Datastream");
                throw new Exception("unable to create new HealthState Datastream");
            }
        }

        return dataStreams;
    }

    private async Task CreateNewDataStream(TreeSenseSensorData dksrTree, Tree tree)
    {
        var healthStateObservedPropertyResponse = await GetOrCreateHealthStateObservedProperty();
        var healthStateTreeSensor = await GetOrCreateTreeSensor(tree);

        var dataStream = new DataStream
        {
            Name = "HealthState",
            Description = "HealthState of a tree",
            ObservationType = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement",
            UnitOfMeasurement = new UnitOfMeasurement
            {
                Name = "HealthState",
                Symbol = "HealthState",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            },
            ObservedArea = new ObservedArea
            {
                Type = "Point",
                Coordinates = new List<double> { double.Parse(dksrTree.Lat), double.Parse(dksrTree.Lng) }
            },
            Thing = new Dictionary<string, string> { { "@iot.id", tree.Id.ToString() } },
            ObservedProperty = new Dictionary<string, string>
                { { "@iot.id", healthStateObservedPropertyResponse.Id.ToString() } },
            Sensor = new Dictionary<string, string> { { "@iot.id", healthStateTreeSensor.Id.ToString() } }
        };

        var response = await FrostApi.DataStreams.PostDataStream(dataStream);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Datastream {dataStream.Name} created successfully");
        else
            Logger.LogError($"Datastream {dataStream.Name} could not be created");
    }

    private async Task<ObservedPropertyResponse> GetOrCreateHealthStateObservedProperty()
    {
        var observedProperties = await FrostApi.ObservedProperties.GetAllObservedProperties();
        var healthStateObservedPropertyResponse =
            observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == "HealthState");
        if (healthStateObservedPropertyResponse == null)
        {
            await CreateNewObservedProperty(new ObservedProperty
            {
                Name = "HealthState",
                Description = "HealthState of a tree",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            });

            observedProperties = await FrostApi.ObservedProperties.GetAllObservedProperties();
            healthStateObservedPropertyResponse =
                observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == "HealthState");
            if (healthStateObservedPropertyResponse == null)
            {
                Logger.LogError("unable to create new HealthState Observed property");
                throw new Exception("unable to create new HealthState Observed property");
            }
        }

        return healthStateObservedPropertyResponse;
    }

    private async Task<SensorResponse> GetOrCreateTreeSensor(Tree tree)
    {
        var sensors = await FrostApi.Sensors.GetAllSensors();
        SensorResponse? healthStateSensorResponse;

        healthStateSensorResponse = sensors.Value.FirstOrDefault(sensor =>
        {
            string? treeId;
            sensor.Properties.TryGetValue("treeId", out treeId);
            sensor.Properties.TryGetValue("id", out treeId);
            if (treeId == tree.Properties.Id.ToString() && sensor.Description == "TreeSensor")
                return true;
            return false;
        });

        if (healthStateSensorResponse == null)
        {
            await CreateNewSensor(new Sensor
            {
                Name = $"TreeSensor for tree {tree.Name}",
                Description = "TreeSensor",
                EncodingType = "application/geo+json",
                Properties = new SensorProps
                {
                    Id = tree.Properties.Id.ToString(),
                    Name = tree.Name
                },
                MetaData = ""
            });

            sensors = await FrostApi.Sensors.GetAllSensors(
                $"?$filter=description eq 'TreeSensor' &$filter= properties/id eq '{tree.Properties.Id}'");
            healthStateSensorResponse = sensors.Value.FirstOrDefault(sensor =>
            {
                string? treeId;
                sensor.Properties.TryGetValue("treeId", out treeId);
                sensor.Properties.TryGetValue("id", out treeId);
                if (treeId == tree.Properties.Id.ToString() && sensor.Description == "TreeSensor")
                    return true;
                return false;
            });

            if (healthStateSensorResponse == null) throw new Exception("unable to create new Tree Sensor ");
        }

        return healthStateSensorResponse;
    }

    private async Task CreateNewSensor(Sensor sensor)
    {
        var response = await FrostApi.Sensors.PostSensor(sensor);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Sensor {sensor.Name} created successfully");
        else
            Logger.LogError($"Sensor {sensor.Name} could not be created");
    }

    private async Task CreateNewObservedProperty(ObservedProperty observedProperty)
    {
        var response = await FrostApi.ObservedProperties.PostObservedProperty(observedProperty);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} created successfully");
        else
            Logger.LogError(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} could not be created");
    }

    private async Task AddObservation(TreeSenseSensorData dksrTree, DataStream healthStateDataStream)
    {
        var observations = await FrostApi.Observations.GetObservationsForDataStream(healthStateDataStream.Id);

        if (observations.Value.Any(observation => observation.PhenomenonTime == dksrTree.Timestamp))
        {
            Logger.LogDebug(
                $"Observation at timestamp {dksrTree.Timestamp} for tree {dksrTree.Id} already exists, skipping");
            return;
        }

        var observation = new Observation
        {
            Result = dksrTree.HealthState,
            PhenomenonTime = dksrTree.Timestamp,
            DataStream = new Dictionary<string, string> { { "@iot.id", healthStateDataStream.Id.ToString() } }
        };

        var response = await FrostApi.Observations.PostObservation(observation);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug(
                $"Observation at timestamp {dksrTree.Timestamp} for tree {dksrTree.Id} created successfully");
        }
        else
        {
            Logger.LogError(
                $"Observation at timestamp {dksrTree.Timestamp} for tree {dksrTree.Id} could not be created");
            Logger.LogError(response.Content.ReadAsStringAsync().Result);
        }
    }

    private async Task<TreesenseResponse> GetDksrTreeData()
    {
        var response =
            await Client.GetAsync(
                Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.TreesenseEndpoint));
        var result = await response.Content.ReadAsAsync<TreesenseResponse>();
        return result;
    }

    private async Task<GetThingsResponse> GetFrostTreeData(int id)
    {
        return await FrostApi.Things.GetAllThings($"?$filter=description eq 'Tree' &$filter= properties/id eq '{id}'");
    }

    private async Task<GetDataStreamsResponse?> GetFrostDataStreamData(int thingId)
    {
        return await FrostApi.DataStreams.GetDataSteamsForThing(thingId);
    }
}