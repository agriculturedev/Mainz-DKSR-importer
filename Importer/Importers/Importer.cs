using System.Net;
using System.Text;
using FrostApi.Models.ObservedProperty;
using FrostApi.Models.Sensor;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.ObservedProperty;
using FrostApi.ResponseModels.Sensor;
using FrostApi.ResponseModels.Thing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class Importer
{
    protected readonly HttpClient Client;
    protected readonly FrostApi.FrostApi FrostApi;
    protected readonly ILogger Logger;
    protected string DataType = "";
    protected string DataStreamName = "";

    protected Importer(ILogger logger, IConfiguration config, string dataType, string dataStreamName)
    {
        FrostApi = new FrostApi.FrostApi(config["FrostBaseUrl"] ?? throw new ArgumentNullException("FrostApiBaseUrl"));
        Username = config["Authentication:Username"] ?? throw new ArgumentNullException("Username");
        Password = config["Authentication:Password"] ?? throw new ArgumentNullException("Username");
        Client = SetupHttpClient();
        Logger = logger;
        DataType = dataType;
        DataStreamName = dataStreamName;
    }

    protected static string Username { get; set; }
    protected static string Password { get; set; }

    private HttpClient SetupHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        };
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,nl;q=0.8");
        client.DefaultRequestHeaders.Add("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}");
        return client;
    }
    
    
    protected async Task<ObservedPropertyResponse> GetOrCreateHealthStateObservedProperty()
    {
        var observedProperties = await FrostApi.ObservedProperties.GetAllObservedProperties();
        var healthStateObservedPropertyResponse =
            observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == DataStreamName);
        if (healthStateObservedPropertyResponse == null)
        {
            await CreateNewObservedProperty(new ObservedProperty
            {
                Name = DataStreamName,
                Description = $"{DataStreamName} state of a {DataType}",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            });

            observedProperties = await FrostApi.ObservedProperties.GetAllObservedProperties();
            healthStateObservedPropertyResponse =
                observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == DataStreamName);
            if (healthStateObservedPropertyResponse == null)
            {
                Logger.LogError($"unable to create new {DataStreamName} Observed property");
                throw new Exception($"unable to create new {DataStreamName} Observed property");
            }
        }

        return healthStateObservedPropertyResponse;
    }

    protected async Task<SensorResponse> GetOrCreateSensor(IThing thing)
    {
        var sensors = await FrostApi.Sensors.GetAllSensors();
        SensorResponse? healthStateSensorResponse;

        healthStateSensorResponse = sensors.Value.FirstOrDefault(sensor =>
        {
            string? parkingLotId;
            sensor.Properties.TryGetValue("id", out parkingLotId);
            if (parkingLotId == thing.Properties.Id.ToString() && sensor.Description == $"{DataType}Sensor")
                return true;
            return false;
        });

        if (healthStateSensorResponse == null)
        {
            await CreateNewSensor(new Sensor
            {
                Name = $"Sensor for {DataType} {thing.Name}",
                Description = $"{DataType}Sensor",
                EncodingType = "application/geo+json",
                Properties = new SensorProps
                {
                    Id = thing.Properties.Id.ToString(),
                    Name = thing.Name
                },
                MetaData = ""
            });

            sensors = await FrostApi.Sensors.GetAllSensors(
                $"?$filter=description eq '{DataType}Sensor' &$filter= properties/id eq '{thing.Properties.Id}'");
            healthStateSensorResponse = sensors.Value.FirstOrDefault(sensor =>
            {
                string? treeId;
                sensor.Properties.TryGetValue("id", out treeId);
                if (treeId == thing.Properties.Id.ToString() && sensor.Description == $"{DataType}Sensor")
                    return true;
                return false;
            });

            if (healthStateSensorResponse == null) throw new Exception($"unable to create new {DataType} Sensor ");
        }

        return healthStateSensorResponse;
    }

    protected async Task CreateNewSensor(Sensor sensor)
    {
        var response = await FrostApi.Sensors.PostSensor(sensor);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Sensor {sensor.Name} created successfully");
        else
            Logger.LogError($"Sensor {sensor.Name} could not be created");
    }

    protected async Task CreateNewObservedProperty(ObservedProperty observedProperty)
    {
        var response = await FrostApi.ObservedProperties.PostObservedProperty(observedProperty);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} created successfully");
        else
            Logger.LogError(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} could not be created");
    }
    
    protected async Task<GetThingsResponse> GetFrostThingData(int id)
    {
        return await FrostApi.Things.GetAllThings($"?$filter=description eq '{DataType}' &$filter= properties/id eq '{id}'");
    }

    protected async Task<GetDataStreamsResponse?> GetFrostDataStreamData(int thingId)
    {
        return await FrostApi.DataStreams.GetDataSteamsForThing(thingId);
    }
    
}