using System.Net;
using System.Text;
using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Location;
using FrostApi.Models.ObservedProperty;
using FrostApi.Models.Sensor;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.ObservedProperty;
using FrostApi.ResponseModels.Sensor;
using FrostApi.ResponseModels.Thing;
using Importer.Configuration;
using Importer.Constants;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class WeatherImporter : IImporter
{
    private Timer _importerTimer;
    private readonly string _dataStreamName;
    private readonly FrostApi.FrostApi _frostApi;
    private readonly HttpClient Client;
    private readonly string DataType;
    private readonly ILogger Logger;
    private static string? Username { get; set; }
    private static string? Password { get; set; }
    private static string SourceUrl { get; set; } = null!;

    public WeatherImporter(ILogger logger, DataSource dataSource)
    {
        Username = dataSource.Username;
        Password = dataSource.Password;
        SourceUrl = dataSource.SourceUrl;
        Client = SetupHttpClient();
        Logger = logger;
        DataType = "hof_owm_connector";
        _dataStreamName = "how_owm_weather";
        _frostApi = new FrostApi.FrostApi(dataSource.DestinationUrl);

        _importerTimer = new Timer(Import, null, 0, 60 * 1000 * 60); // every hour
        // _importerTimer = new Timer(Import, null, 0, 60 * 1000); // every hour
        Logger.LogInformation($"{DateTime.Now} - Starting {DataType} Sensor Data Collection");
    }

    protected async void Import(object? _)
    {
        try
        {
            Logger.LogInformation($"{DateTime.Now} - Updating {DataType} Data...");
            var data = await GetData();
            foreach (var weather in data)
            {
                try
                {
                    var thing = Mappers.MapDksrResponse(weather, DataType);
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

    private async Task<List<WeatherSensorData>> GetData()
    {
        try
        {
            var url = Endpoints.GetAuthenticatedEndpointUrl(Username, Password, SourceUrl);
            var response = await Client.GetAsync(url);
            var result = await response.Content.ReadAsAsync<WeatherSensorDataWrapper>();
            return result.SensorData.ToList();
        }
        catch (Exception e)
        {
            Logger.LogError("Getting data failed, returning empty response");
            throw new Exception($"{DateTime.Now} - {e}");
        }
    }

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

    protected async void UpdateThing(Thing thing)
    {
        // Thing thing;
        // thing = Mappers.MapDksrResponse(dksResponse, DataType);
        var frostThing = await GetFrostThingData(thing.Properties["Id"]);

        if (frostThing.Value.Count == 0)
        {
            await CreateNewThing(thing);
            frostThing = await GetFrostThingData(thing.Properties["Id"]);
        }

        if (frostThing.Value.Count < 1)
            throw new Exception($"Creating new thing with id {thing.Properties["Id"]} seems to have failed...");

        thing.Id = frostThing.Value.First().Id;
        await Update(thing);
    }

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


    protected async Task CreateLocationIfNotExists(Thing thing)
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

    protected async Task CreateNewLocation(Thing thing)
    {
        var location = new ThingLocation
        {
            Name = "Location",
            Description = $"Location of a {DataType}",
            EncodingType = "application/geo+json",
            Location = new LocationProperties
            {
                Type = "Point",
                Coordinates = new List<double> { thing.Lon, thing.Lat }
            },
            Things = new List<Dictionary<string, string>> { new() { { "@iot.id", thing.Id.ToString() } } }
        };

        var response = await _frostApi.Locations.PostLocation(location);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Location {location.Name} created successfully");
        else
            Logger.LogError($"Location {location.Name} could not be created");
    }


    protected async Task<SensorResponse> GetOrCreateSensor(Thing thing)
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

    protected async Task CreateNewSensor(Sensor sensor)
    {
        var response = await _frostApi.Sensors.PostSensor(sensor);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Sensor {sensor.Name} created successfully");
        else
            Logger.LogError($"Sensor {sensor.Name} could not be created");
    }


    protected async Task<ObservedPropertyResponse> GetOrCreateObservedProperty()
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

    protected async Task CreateNewObservedProperty(ObservedProperty observedProperty)
    {
        var response = await _frostApi.ObservedProperties.PostObservedProperty(observedProperty);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} created successfully");
        else
            Logger.LogError(
                $"ObservedProperty {observedProperty.Name} with Id {observedProperty.Id} could not be created");
    }


    protected async Task AddObservation(Thing thing, DataStream dataStream)
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
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
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


    protected async Task<GetDataStreamsResponse> GetOrCreateDataStream(Thing thing)
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

    protected async Task CreateNewDataStream(Thing thing)
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
            Sensor = new Dictionary<string, string> { { "@iot.id", sensor.Id.ToString() } },
            Properties = new Dictionary<string, string>
            {
                { "countrycode", "Two letter country code" },
                { "temp", "Current average temperature in the city in C" },
                { "temp_min", "Current minimum measured temperature in the city in C" },
                { "temp_max", "Current maximum measured temperature in the city in C" },
                { "pressure", "Pressure in hPa" },
                { "humidity", "Relative humidity in percentages (0-100)" },
                { "sunrise", "Time of next sunrise in UTC?" },
                { "sunset", "Time of next sunset in UTC?" },
                { "lat", "Latitude" },
                { "lon", "Longitude" },
                { "wind_speed", "Speed of the wind" },
                { "wind_deg", "The deg of the wind" },
                { "clouds", "Number of the clouds" },
                { "rain", "rain indicator" },
                { "visibility", "Visibility" },
                { "weather_id", "Weather condition ID. See http://openweathermap.org/weather-conditions for the list of IDs" },
                { "weather_icon", "URL to icon that describes current weather condition"},
                { "city", "City Name"}
            }
        };

        var response = await _frostApi.DataStreams.PostDataStream(dataStream);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Datastream {dataStream.Name} created successfully");
        else
            Logger.LogError($"Datastream {dataStream.Name} could not be created");
    }

    protected Task<GetThingsResponse> GetFrostThingData(string id)
    {
        return _frostApi.Things.GetAllThings($"?$filter=description eq '{DataType}' &$filter= properties/Id eq '{id}'");
    }

    protected Task<GetDataStreamsResponse?> GetFrostDataStreamData(int thingId)
    {
        return _frostApi.DataStreams.GetDataSteamsForThing(thingId);
    }
}