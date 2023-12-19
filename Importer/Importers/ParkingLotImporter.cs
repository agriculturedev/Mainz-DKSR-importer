using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Location;
using FrostApi.Models.Observation;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.Thing;
using Importer.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class ParkingLotImporter : Importer
{
    private Timer _importerTimer;

    public ParkingLotImporter(ILogger logger, IConfiguration config) : base(logger, config, "ParkingLot", "Occupancy")
    {
        Logger.LogInformation($"Starting {DataType} Sensor Data Collection");
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
            Logger.LogInformation($"Updating {DataType} Data...");
            var data = await GetDksrData();
            foreach (var parkingLot in data.SensorData)
                try
                {
                    var frostParkingLot = await GetFrostThingData(parkingLot.ParkingSpaceId);
                    if (frostParkingLot.Value.Count == 0)
                    {
                        await CreateNewParkingLot(parkingLot);
                        frostParkingLot = await GetFrostThingData(parkingLot.ParkingSpaceId);
                    }

                    await Update(parkingLot, frostParkingLot);
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

    private Task CreateNewParkingLot(ParkingLotSensorData dksrParkingLot)
    {
        var parkingLot = Mappers.MapDksrResponse(dksrParkingLot);

        Logger.LogDebug($"{DataType} {parkingLot.Name} with Id {parkingLot.Properties.Id} not found in Frost, creating new");
        var postResponse = FrostApi.Things.PostThing(parkingLot).Result;
        if (postResponse.IsSuccessStatusCode)
            Logger.LogDebug($"{DataType} {parkingLot.Name} with Id {parkingLot.Properties.Id} created successfully");
        else
            Logger.LogError($"{DataType} {parkingLot.Name} with Id {parkingLot.Properties.Id} could not be created");
        return Task.CompletedTask;
    }

    private async Task Update(ParkingLotSensorData dksrParkingLot, GetThingsResponse frostParkingLot)
    {
        var parkingLot = Mappers.MapDksrResponse(dksrParkingLot);

        Logger.LogDebug($"{DataType} {dksrParkingLot.Sid} with Id {dksrParkingLot.ParkingSpaceId} found in Frost, updating...");

        parkingLot.Id = frostParkingLot.Value.First().Id;
        var response = await FrostApi.Things.UpdateThing(parkingLot);

        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug($"{DataType} {parkingLot.Name} with Id {parkingLot.Properties.Id} updated successfully");
            await UpdateDataStream(dksrParkingLot, parkingLot);
        }
        else
        {
            Logger.LogError($"{DataType} {parkingLot.Name} with Id {parkingLot.Properties.Id} could not be updated");
        }
    }

    private async Task UpdateDataStream(ParkingLotSensorData dksrParkingLot, IThing thing)
    {
        var dataStreams = await GetOrCreateDataStream(dksrParkingLot, thing);
        var healthStateDataStream =
            Mappers.MapFrostResponseToDataStream(dataStreams.Value.Find(dataStream =>
                dataStream?.Name == DataStreamName));

        await CreateLocationIfNotExists(dksrParkingLot, thing);
        await AddObservation(dksrParkingLot, healthStateDataStream);
    }

    private async Task CreateLocationIfNotExists(ParkingLotSensorData dksrParkingLot, IThing thing)
    {
        var locations = await FrostApi.Locations.GetLocationsForThing(thing.Id);
        if (locations?.Value == null || locations.Value.Count == 0)
        {
            await CreateNewLocation(dksrParkingLot, thing);

            locations = await FrostApi.Locations.GetLocationsForThing(thing.Id);
            if (locations?.Value == null || locations.Value.Count == 0)
            {
                Logger.LogError("unable to create new Location");
                throw new Exception("unable to create new Location");
            }
        }
    }

    private async Task CreateNewLocation(ParkingLotSensorData dksrParkingLot, IThing thing)
    {
        var location = new ThingLocation
        {
            Name = "Location",
            Description = $"Location of a {DataType}",
            EncodingType = "application/geo+json",
            Location = new LocationProperties
            {
                Type = "Point",
                Coordinates = new List<string> { dksrParkingLot.Lat.ToString(), dksrParkingLot.Lon.ToString() }
            },
            Things = new List<Dictionary<string, string>> { new() { { "@iot.id", thing.Id.ToString() } } }
        };

        var response = await FrostApi.Locations.PostLocation(location);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Location {location.Name} created successfully");
        else
            Logger.LogError($"Location {location.Name} could not be created");
    }

    private async Task<GetDataStreamsResponse> GetOrCreateDataStream(ParkingLotSensorData dksrParkingLot, IThing thing)
    {
        var dataStreams = await GetFrostDataStreamData(thing.Id);
        if (dataStreams?.Value == null || dataStreams.Value.Count == 0)
        {
            await CreateNewDataStream(dksrParkingLot, thing);

            dataStreams = await GetFrostDataStreamData(thing.Id);
            if (dataStreams?.Value == null || dataStreams.Value.Count == 0)
            {
                Logger.LogError($"unable to create new {DataStreamName} Datastream");
                throw new Exception($"unable to create new {DataStreamName} Datastream");
            }
        }

        return dataStreams;
    }

    private async Task CreateNewDataStream(ParkingLotSensorData dksrParkingLot, IThing thing)
    {
        var observedPropertyResponse = await GetOrCreateHealthStateObservedProperty();
        var sensor = await GetOrCreateSensor(thing);

        var dataStream = new DataStream
        {
            Name = $"{DataStreamName}",
            Description = $"{DataStreamName} status of a {DataType}",
            ObservationType = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement",
            UnitOfMeasurement = new UnitOfMeasurement
            {
                Name = $"{DataStreamName}",
                Symbol = $"{DataStreamName}",
                Definition = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement"
            },
            ObservedArea = new ObservedArea
            {
                Type = "Point",
                Coordinates = new List<double> { dksrParkingLot.Lat, dksrParkingLot.Lon }
            },
            Thing = new Dictionary<string, string> { { "@iot.id", thing.Id.ToString() } },
            ObservedProperty = new Dictionary<string, string>
                { { "@iot.id", observedPropertyResponse.Id.ToString() } },
            Sensor = new Dictionary<string, string> { { "@iot.id", sensor.Id.ToString() } }
        };

        var response = await FrostApi.DataStreams.PostDataStream(dataStream);
        if (response.IsSuccessStatusCode)
            Logger.LogDebug($"Datastream {dataStream.Name} created successfully");
        else
            Logger.LogError($"Datastream {dataStream.Name} could not be created");
    }

    

    private async Task AddObservation(ParkingLotSensorData dksrParkingLot, DataStream dataStream)
    {
        var observations = await FrostApi.Observations.GetObservationsForDataStream(dataStream.Id);

        if (observations.Value.Any(observation => observation.PhenomenonTime == dksrParkingLot.Timestamp))
        {
            Logger.LogDebug(
                $"Observation at timestamp {dksrParkingLot.Timestamp} for {DataType} {dksrParkingLot.ParkingSpaceId} already exists, skipping");
            return;
        }

        var observation = new Observation
        {
            Result = dksrParkingLot.Occupied.ToString(),
            PhenomenonTime = dksrParkingLot.Timestamp,
            DataStream = new Dictionary<string, string> { { "@iot.id", dataStream.Id.ToString() } }
        };

        var response = await FrostApi.Observations.PostObservation(observation);
        if (response.IsSuccessStatusCode)
        {
            Logger.LogDebug(
                $"Observation at timestamp {dksrParkingLot.Timestamp} for {DataType} {dksrParkingLot.ParkingSpaceId} created successfully");
        }
        else
        {
            Logger.LogError(
                $"Observation at timestamp {dksrParkingLot.Timestamp} for {DataType} {dksrParkingLot.ParkingSpaceId} could not be created");
            Logger.LogError(response.Content.ReadAsStringAsync().Result);
        }
    }

    private async Task<ParkingLotResponse> GetDksrData()
    {
        var response =
            await Client.GetAsync(
                Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.ParkingLotEndpoint));
        var result = await response.Content.ReadAsAsync<ParkingLotResponse>();
        return result;
    }

  
}