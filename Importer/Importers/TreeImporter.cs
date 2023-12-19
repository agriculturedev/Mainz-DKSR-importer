using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Observation;
using FrostApi.Models.ObservedProperty;
using FrostApi.Models.Sensor;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.FeaturesOfInterest;
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
    private Timer importerTimer;
    public TreeImporter(ILogger logger, IConfiguration config) : base(logger, config)
    {
    }

    public void Start()
    {
        Logger.LogInformation("Starting TreeSense Sensor Data Collection");
        importerTimer = new Timer(_ => Import(), null, 0, 60 * 1000 * 60);
    }

    public async Task Import()
    {
        try
        {
            var data = await GetDksrTreeData();
            foreach (var dksrTree in data.SensorData)
            {
                try
                {
                    var FrostTree = await GetFrostTreeData(int.Parse(dksrTree.Id));
                    if (FrostTree.Value.Count == 0)
                    {
                        await CreateNewTree(dksrTree, FrostTree);
                        FrostTree = await GetFrostTreeData(int.Parse(dksrTree.Id));
                    }
                    
                    await UpdateTree(dksrTree, FrostTree);
                    
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }

            ;
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString());
        }
    }

    public async Task CreateNewTree(TreeSenseSensorData dksrTree, GetThingsResponse FrostTree)
    {
        var tree = TreeMapper.MapDksrResponseToTree(dksrTree);

        Logger.LogInformation($"Tree {tree.Name} with Id {tree.Properties.Id} not found in Frost, creating new tree");
        var postResponse = FrostApi.Things.PostThing(tree).Result;
        if (postResponse.IsSuccessStatusCode)
        {
            Logger.LogInformation($"Tree {tree.Name} with Id {tree.Properties.Id} created successfully");
        }
        else
        {
            Logger.LogError($"Tree {tree.Name} with Id {tree.Properties.Id} could not be created");
        }
    }

    public async Task UpdateTree(TreeSenseSensorData dksrTree, GetThingsResponse FrostTree)
    {
        var tree = TreeMapper.MapDksrResponseToTree(dksrTree);

        Logger.LogInformation($"Tree {dksrTree.Name} with Id {dksrTree.Id} found in Frost, updating tree");

        tree.Id = FrostTree.Value.First().Id;
        var response = await FrostApi.Things.UpdateThing(tree);

        if (response.IsSuccessStatusCode)
        {
            Logger.LogInformation($"Tree {tree.Name} with Id {tree.Properties.Id} updated successfully");
            await UpdateDataStream(dksrTree, tree);
        }
        else
        {
            Logger.LogError($"Tree {tree.Name} with Id {tree.Properties.Id} could not be updated");
        }
    }

    public async Task UpdateDataStream(TreeSenseSensorData dksrTree, Tree tree)
    {
        var dataStreams = await GetFrostDataStreamData(tree.Id);
        if (dataStreams.Value == null || dataStreams.Value.Count == 0)
        {
            await CreateNewDataStream(dksrTree, tree);
        }
        
        dataStreams = await GetFrostDataStreamData(tree.Id);

        var healthStateDataStream =
            DataStreamMapper.MapFrostResponseToDataStream(dataStreams.Value.Find(dataStream => dataStream.Name == "HealthState"));
        
        
        // get or create feature of interest for specific tree
        
        // var featureOfInterestResponse = await FrostApi.FeatureOfInterest.GetAllFeaturesOfInterest($"?$filter=description eq 'Tree' &$filter= properties/id eq '{tree.Properties.Id}'");
        
        await UpdateObservation(dksrTree, healthStateDataStream);
    }

    public async Task CreateNewDataStream(TreeSenseSensorData dksrTree, Tree tree)
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
            ObservedArea = new ObservedArea()
            {
                Type = "Point",
                Coordinates = new List<double> { double.Parse(dksrTree.Lat), double.Parse(dksrTree.Lng) }
            },
            Thing = new Dictionary<string, string> { { "@iot.id", tree.Id.ToString() } },
            ObservedProperty = new Dictionary<string, string> { { "@iot.id", healthStateObservedPropertyResponse.Id.ToString() } },
            Sensor = new Dictionary<string, string> { { "@iot.id", healthStateTreeSensor.Id.ToString() } }
        };
        
        var response = await FrostApi.DataStreams.PostDataStream(dataStream);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogInformation($"Datastream {dataStream.Name} created successfully");
        }
        else
        {
            Logger.LogError($"Datastream {dataStream.Name} could not be created");
        }
    }
    
    public async Task<ObservedPropertyResponse> GetOrCreateHealthStateObservedProperty()
    {
        var observedProperties = await FrostApi.ObservedProperties.GetAllObservedProperties();
        var healthStateObservedPropertyResponse = observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == "HealthState");
        if (healthStateObservedPropertyResponse == null)
        {
            await CreateNewObservedProperty(new ObservedProperty
            {
                Name = "HealthState",
                Description = "HealthState of a tree",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            });
            
            observedProperties = await FrostApi.ObservedProperties.GetAllObservedProperties();
            healthStateObservedPropertyResponse = observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == "HealthState");
            if (healthStateObservedPropertyResponse == null)
            {
                Logger.LogError($"unable to create new HealthState Observed property");
                throw new Exception("unable to create new HealthState Observed property");
            }
        }

        return healthStateObservedPropertyResponse;
    }
    
    public async Task<SensorResponse> GetOrCreateTreeSensor(Tree tree)
    {
        var sensors = await FrostApi.Sensors.GetAllSensors();
        SensorResponse? healthStateSensorResponse;

        healthStateSensorResponse = sensors.Value.FirstOrDefault(sensor =>
        {
            string treeId;
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
                Properties = new SensorProps()
                {
                    Id = tree.Properties.Id.ToString(),
                    Name = tree.Name,
                },
                MetaData = ""
            });
            
            sensors = await FrostApi.Sensors.GetAllSensors($"?$filter=description eq 'TreeSensor' &$filter= properties/id eq '{tree.Properties.Id}'");
            healthStateSensorResponse = sensors.Value.FirstOrDefault(sensor =>
            {
                string treeId;
                sensor.Properties.TryGetValue("treeId", out treeId);
                sensor.Properties.TryGetValue("id", out treeId);
                if (treeId == tree.Properties.Id.ToString() && sensor.Description == "TreeSensor")
                    return true;
                return false;
            });
            
            if (healthStateSensorResponse == null)
            {
                throw new Exception("unable to create new Tree Sensor ");
            }
        }

        return healthStateSensorResponse;
    }
    
    public async Task CreateNewSensor(Sensor sensor)
    {
        var response = await FrostApi.Sensors.PostSensor(sensor);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogInformation($"Sensor {sensor.Name} created successfully");
        }
        else
        {
            Logger.LogError($"Sensor {sensor.Name} could not be created");
        }
    }
    
    public async Task CreateNewObservedProperty(ObservedProperty observedProperty)
    {
        var response = await FrostApi.ObservedProperties.PostObservedProperty(observedProperty);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogInformation($"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} created successfully");
        }
        else
        {
            Logger.LogError($"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} could not be created");
        }
    }

    public async Task UpdateObservation(TreeSenseSensorData dksrTree, DataStream healthStateDataStream)
    {
        var observations = await FrostApi.Observations.GetObservationsForDataStream(healthStateDataStream.Id);

        if (observations.Value.Any(observation => observation.PhenomenonTime == dksrTree.Timestamp))
        {
            Logger.LogInformation($"Observation at timestamp {dksrTree.Timestamp} for tree {dksrTree.Id} already exists, skipping");
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
            Logger.LogInformation($"Observation at timestamp {dksrTree.Timestamp} for tree {dksrTree.Id} created successfully");
        }
        else
        {
            Logger.LogError($"Observation at timestamp {dksrTree.Timestamp} for tree {dksrTree.Id} could not be created");
        }
    }

    public async Task<TreesenseResponse> GetDksrTreeData()
    {
        var response = await Client.GetAsync(Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.TreesenseEndpoint));
        var result = await response.Content.ReadAsAsync<TreesenseResponse>();
        return result;
    }

    public async Task<GetThingsResponse> GetFrostTreeData(int id)
    {
        return await FrostApi.Things.GetAllThings($"?$filter=description eq 'Tree' &$filter= properties/id eq '{id}'");
    }

    public async Task<GetDataStreamsResponse?> GetFrostDataStreamData(int thingId)
    {
        return await FrostApi.DataStreams.GetDataSteamsForThing(thingId);
    }
}