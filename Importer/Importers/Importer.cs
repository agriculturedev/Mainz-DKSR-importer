using System.Net;
using System.Text;
using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Location;
using FrostApi.Models.Observation;
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

public abstract class Importer
{
    private readonly FrostApi.FrostApi _frostApi;
    protected readonly HttpClient Client;
    protected readonly string DataType;
    protected readonly ILogger Logger;

    protected Importer(ILogger logger, string dataType, DataSource source)
    {
        _frostApi = new FrostApi.FrostApi(source.DestinationUrl);

        Username = source.Username;
        Password = source.Password;
        SourceUrl = source.SourceUrl;
        Client = SetupHttpClient();
        Logger = logger;
        DataType = dataType;

        Logger.LogInformation($"{DateTime.Now} - Starting {DataType} Sensor Data Collection");
    }

    protected static string? Username { get; set; }
    protected static string? Password { get; set; }
    protected static string SourceUrl { get; set; } = null!;

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

    protected async Task<List<T>> GetData<T>() where T : IDksrResponse
    {
        try
        {
            var url = Endpoints.GetAuthenticatedEndpointUrl(Username, Password, SourceUrl);
            var response = await Client.GetAsync(url);
            var result = await response.Content.ReadAsAsync<List<T>>();
            return result;
        }
        catch (Exception e)
        {
            Logger.LogError("Getting data failed, returning empty response");
            throw new Exception($"{DateTime.Now} - {e}");
        }
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

            await CreateLocationIfNotExists(thing);

            foreach (var observation in thing.LatestObservations)
            {
                var dataStream = await GetOrCreateDataStream(thing, observation);
                var mappedStream =
                    Mappers.MapFrostResponseToDataStream(dataStream.Value.Find(stream =>
                        stream?.Name == observation.Name));

                await AddObservation(observation, thing.Id, mappedStream);
            }
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
                sensor.Properties.TryGetValue("id", out var sensorId);
                if (sensorId == thing.Properties["Id"] && sensor.Description == $"{DataType}Sensor")
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


    protected async Task<ObservedPropertyResponse> GetOrCreateObservedProperty(Observation observation)
    {
        var observedProperties = await _frostApi.ObservedProperties.GetAllObservedProperties();

        var getObserverPropFromResponse = (GetObservedPropertiesResponse? response)
            => response?.Value.FirstOrDefault(op => op.Name == observation.ObservationType.Name);

        var currentObservationObservedPropertyResponse =
            getObserverPropFromResponse(observedProperties);

        if (currentObservationObservedPropertyResponse == null)
        {
            await CreateNewObservedProperty(new ObservedProperty
            {
                Name = observation.ObservationType.Name,
                Description = observation.ObservationType.Description,
                Definition = observation.ObservationType.Definition,
            });

            observedProperties = await _frostApi.ObservedProperties.GetAllObservedProperties();
            currentObservationObservedPropertyResponse =
                getObserverPropFromResponse(observedProperties);

            if (currentObservationObservedPropertyResponse == null)
            {
                Logger.LogError($"unable to create new {observation.Name} Observed property");
                throw new Exception($"unable to create new {observation.Name} Observed property");
            }
        }

        return currentObservationObservedPropertyResponse;
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


    protected async Task AddObservation(Observation observation, int thingId, DataStream dataStream)
    {
        var observations = await _frostApi.Observations.GetObservationsForDataStream(dataStream.Id);

        if (observations.Value.Any(existingObs => existingObs.PhenomenonTime == observation.PhenomenonTime))
        {
            Logger.LogDebug(
                $"Observation at timestamp {observation.PhenomenonTime} for {DataType} - {dataStream.Name} already exists, skipping");
            return;
        }

        observation.DataStream = new Dictionary<string, string> { { "@iot.id", dataStream.Id.ToString() } };

        var response = await _frostApi.Observations.PostObservation(observation);
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug(
                $"Observation at timestamp {observation.PhenomenonTime} for {DataType} {thingId} created successfully");
        }
        else
        {
            Logger.LogError(
                $"Observation at timestamp {observation.PhenomenonTime} for {DataType} {thingId} could not be created");
            Logger.LogError(response.Content.ReadAsStringAsync().Result);
        }
    }

    protected async Task<GetDataStreamsResponse> GetOrCreateDataStream(Thing thing, Observation observation)
    {
        var dataStreams = await GetFrostDataStreamData(thing.Id);
        if (dataStreams?.Value == null || dataStreams.Value.Count < thing.LatestObservations.Count) // TODO@JOREN: probably needs smarter checking to see which streams need to be created
        {
            await CreateNewDataStream(thing, observation);

            dataStreams = await GetFrostDataStreamData(thing.Id);
            if (dataStreams?.Value == null) // TODO@JOREN: Also should be smarter
            {
                Logger.LogError($"unable to create new {observation.Name} Datastream");
                throw new Exception($"unable to create new {observation.Name} Datastream");
            }
        }

        return dataStreams;
    }

    protected async Task CreateNewDataStream(Thing thing, Observation observation)
    {
        var observedPropertyResponse = await GetOrCreateObservedProperty(observation);
        var sensor = await GetOrCreateSensor(thing);

        var dataStream = new DataStream
        {
            Name = $"{observation.Name}",
            Description = $"{observation.Name} status of a {DataType}",
            ObservationType = $"{observation.ObservationType.Name}",
            UnitOfMeasurement = new UnitOfMeasurement
            {
                Name = $"{observation.UnitOfMeasurementName}",
                Symbol = $"{observation.UnitOfMeasurementSymbol}",
                Definition = $"{observation.UnitOfMeasurementDefinition}"
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

    protected Task<GetThingsResponse> GetFrostThingData(string id)
    {
        return _frostApi.Things.GetAllThings($"?$filter=description eq '{DataType}' &$filter= properties/Id eq '{id}'");
    }

    protected Task<GetDataStreamsResponse?> GetFrostDataStreamData(int thingId)
    {
        return _frostApi.DataStreams.GetDataSteamsForThing(thingId);
    }
}