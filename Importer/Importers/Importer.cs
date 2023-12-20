using System.Globalization;
using System.Net;
using System.Text;
using FrostApi.Models.DataStream;
using FrostApi.Models.Location;
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

public abstract class Importer
{
    private readonly string _dataStreamName;
    protected readonly HttpClient Client;
    protected readonly string DataType;
    private readonly FrostApi.FrostApi _frostApi;
    protected readonly ILogger Logger;

    protected Importer(ILogger logger, IConfiguration config, string dataType, string dataStreamName)
    {
        _frostApi = new FrostApi.FrostApi(config["FrostBaseUrl"] ?? throw new ArgumentNullException("FrostBaseUrl"));
        Username = config["Authentication:Username"] ?? throw new ArgumentNullException("Username");
        Password = config["Authentication:Password"] ?? throw new ArgumentNullException("Password");
        Client = SetupHttpClient();
        Logger = logger;
        DataType = dataType;
        _dataStreamName = dataStreamName;
        
        Logger.LogInformation($"Starting {DataType} Sensor Data Collection");
    }

    protected static string? Username { get; set; }
    protected static string? Password { get; set; }

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

    protected abstract void Import(object? _);

    protected Task CreateNewThing(Thing thing)
    {
        Logger.LogDebug($"{DataType} {thing.Name} with Id {thing.Properties["Id"]} not found in Frost, creating new");
        var postResponse = _frostApi.Things.PostThing(thing).Result;
        if (postResponse.IsSuccessStatusCode)
            Logger.LogDebug($"{DataType} {thing.Name} with Id {thing.Properties["Id"]} created successfully");
        else
            Logger.LogError($"{DataType} {thing.Name} with Id {thing.Properties["Id"]} could not be created");
        return Task.CompletedTask;
    }
    protected async Task Update(Thing thing)
    {
        Logger.LogDebug($"{DataType} {thing.Name} with Id {thing.Id} found in Frost, updating...");
        var response = await _frostApi.Things.UpdateThing(thing);

        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug($"{DataType} {thing.Name} with Id {thing.Properties["Id"]} updated successfully");
            var dataStreams = await GetOrCreateDataStream(thing);
            var dataStream =
                Mappers.MapFrostResponseToDataStream(dataStreams.Value.Find(dataStream =>
                    dataStream?.Name == _dataStreamName));

            await CreateLocationIfNotExists(thing);
            await AddObservation(thing, dataStream);
        }
        else
        {
            Logger.LogError($"{DataType} {thing.Name} with Id {thing.Properties["Id"]} could not be updated");
        }
    }

    
    private async Task CreateLocationIfNotExists(Thing thing)
    {
        var locations = await _frostApi.Locations.GetLocationsForThing(thing.Id);
        if (locations?.Value == null || locations.Value.Count == 0)
        {
            await CreateNewLocation(thing);

            locations = await _frostApi.Locations.GetLocationsForThing(thing.Id);
            if (locations?.Value == null || locations.Value.Count == 0)
            {
                Logger.LogError("unable to create new Location");
                throw new Exception("unable to create new Location");
            }
        }
    }
    private async Task CreateNewLocation(Thing thing)
    {
        var location = new ThingLocation
        {
            Name = "Location",
            Description = $"Location of a {DataType}",
            EncodingType = "application/geo+json",
            Location = new LocationProperties
            {
                Type = "Point",
                Coordinates = new List<string> { thing.Lat.ToString(CultureInfo.InvariantCulture), thing.Lon.ToString(CultureInfo.InvariantCulture) }
            },
            Things = new List<Dictionary<string, string>> { new() { { "@iot.id", thing.Id.ToString() } } }
        };

        var response = await _frostApi.Locations.PostLocation(location);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Location {location.Name} created successfully");
        else
            Logger.LogError($"Location {location.Name} could not be created");
    }


    private async Task<SensorResponse> GetOrCreateSensor(Thing thing)
    {
        var sensors = await _frostApi.Sensors.GetAllSensors();
        SensorResponse? sensorResponse;

        sensorResponse = sensors.Value.FirstOrDefault(sensor =>
        {
            string? sensorThingId;
            sensor.Properties.TryGetValue("id", out sensorThingId);
            if (sensorThingId == thing.Properties["Id"] && sensor.Description == $"{DataType}Sensor")
                return true;
            return false;
        });

        if (sensorResponse == null)
        {
            await CreateNewSensor(new Sensor
            {
                Name = $"Sensor for {DataType} {thing.Name}",
                Description = $"{DataType}Sensor",
                EncodingType = "application/geo+json",
                Properties = new SensorProps
                {
                    Id = thing.Properties["Id"],
                    Name = thing.Name
                },
                MetaData = ""
            });

            sensors = await _frostApi.Sensors.GetAllSensors(
                $"?$filter=description eq '{DataType}Sensor' &$filter= properties/id eq '{thing.Properties["Id"]}'");
            sensorResponse = sensors.Value.FirstOrDefault(sensor =>
            {
                string? treeId;
                sensor.Properties.TryGetValue("id", out treeId);
                if (treeId == thing.Properties["Id"] && sensor.Description == $"{DataType}Sensor")
                    return true;
                return false;
            });

            if (sensorResponse == null) throw new Exception($"unable to create new {DataType} Sensor ");
        }

        return sensorResponse;
    }
    private async Task CreateNewSensor(Sensor sensor)
    {
        var response = await _frostApi.Sensors.PostSensor(sensor);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Sensor {sensor.Name} created successfully");
        else
            Logger.LogError($"Sensor {sensor.Name} could not be created");
    }


    private async Task<ObservedPropertyResponse> GetOrCreateObservedProperty()
    {
        var observedProperties = await _frostApi.ObservedProperties.GetAllObservedProperties();
        var healthStateObservedPropertyResponse =
            observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == _dataStreamName);
        if (healthStateObservedPropertyResponse == null)
        {
            await CreateNewObservedProperty(new ObservedProperty
            {
                Name = _dataStreamName,
                Description = $"{_dataStreamName} state of a {DataType}",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            });

            observedProperties = await _frostApi.ObservedProperties.GetAllObservedProperties();
            healthStateObservedPropertyResponse =
                observedProperties.Value.FirstOrDefault(observedProperty => observedProperty.Name == _dataStreamName);
            if (healthStateObservedPropertyResponse == null)
            {
                Logger.LogError($"unable to create new {_dataStreamName} Observed property");
                throw new Exception($"unable to create new {_dataStreamName} Observed property");
            }
        }

        return healthStateObservedPropertyResponse;
    }
    private async Task CreateNewObservedProperty(ObservedProperty observedProperty)
    {
        var response = await _frostApi.ObservedProperties.PostObservedProperty(observedProperty);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} created successfully");
        else
            Logger.LogError(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} could not be created");
    }

    
    private async Task AddObservation(Thing thing, DataStream dataStream)
    {
        var observations = await _frostApi.Observations.GetObservationsForDataStream(dataStream.Id);

        if (observations.Value.Any(observation => observation.PhenomenonTime == thing.LatestObservation.PhenomenonTime))
        {
            Logger.LogDebug(
                $"Observation at timestamp {thing.LatestObservation.PhenomenonTime} for {DataType} {thing.Properties["Id"]} already exists, skipping");
            return;
        }

        thing.LatestObservation.DataStream = new Dictionary<string, string> { { "@iot.id", dataStream.Id.ToString() } };

        var response = await _frostApi.Observations.PostObservation(thing.LatestObservation);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug(
                $"Observation at timestamp {thing.LatestObservation.PhenomenonTime} for {DataType} {thing.Id} created successfully");
        }
        else
        {
            Logger.LogError(
                $"Observation at timestamp {thing.LatestObservation.PhenomenonTime} for {DataType} {thing.Id} could not be created");
            Logger.LogError(response.Content.ReadAsStringAsync().Result);
        }
    }
    
    
    private async Task<GetDataStreamsResponse> GetOrCreateDataStream(Thing thing)
    {
        var dataStreams = await GetFrostDataStreamData(thing.Id);
        if (dataStreams?.Value == null || dataStreams.Value.Count == 0)
        {
            await CreateNewDataStream(thing);

            dataStreams = await GetFrostDataStreamData(thing.Id);
            if (dataStreams?.Value == null || dataStreams.Value.Count == 0)
            {
                Logger.LogError($"unable to create new {_dataStreamName} Datastream");
                throw new Exception($"unable to create new {_dataStreamName} Datastream");
            }
        }

        return dataStreams;
    }
    private async Task CreateNewDataStream(Thing thing)
    {
        var observedPropertyResponse = await GetOrCreateObservedProperty();
        var sensor = await GetOrCreateSensor(thing);

        var dataStream = new DataStream
        {
            Name = $"{_dataStreamName}",
            Description = $"{_dataStreamName} status of a {DataType}",
            ObservationType = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement",
            UnitOfMeasurement = new UnitOfMeasurement
            {
                Name = $"{_dataStreamName}",
                Symbol = $"{_dataStreamName}",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            },
            ObservedArea = new ObservedArea
            {
                Type = "Point",
                Coordinates = new List<double> { thing.Lat, thing.Lon }
            },
            Thing = new Dictionary<string, string> { { "@iot.id", thing.Id.ToString() } },
            ObservedProperty = new Dictionary<string, string>
                { { "@iot.id", observedPropertyResponse.Id.ToString() } },
            Sensor = new Dictionary<string, string> { { "@iot.id", sensor.Id.ToString() } }
        };

        var response = await _frostApi.DataStreams.PostDataStream(dataStream);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Datastream {dataStream.Name} created successfully");
        else
            Logger.LogError($"Datastream {dataStream.Name} could not be created");
    }

    protected Task<GetThingsResponse> GetFrostThingData(int id)
    {
        return _frostApi.Things.GetAllThings($"?$filter=description eq '{DataType}' &$filter= properties/Id eq '{id}'");
    }
    
    protected Task<GetThingsResponse> GetFrostThingData(string id)
    {
        return _frostApi.Things.GetAllThings($"?$filter=description eq '{DataType}' &$filter= properties/Id eq '{id}'");
    }

    private Task<GetDataStreamsResponse?> GetFrostDataStreamData(int thingId)
    {
        return _frostApi.DataStreams.GetDataSteamsForThing(thingId);
    }
}