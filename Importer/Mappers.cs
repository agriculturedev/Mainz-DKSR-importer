using System.Globalization;
using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Observation;
using FrostApi.Models.ObservedProperty;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;

namespace Importer;

public static class Mappers
{
    public static Thing MapDksrResponse(TreeSenseSensorData treeData, string dataType)
    {
        var properties = new Dictionary<string, string> { { "Id", treeData.Sid } };

        var observation = new Observation
        {
            Name = "HealthState",
            Result = treeData.HealthState,
            ObservationType = ObservedProperty.HealthState(),
            PhenomenonTime = treeData.Timestamp,
            ResultTime = treeData.Timestamp
        };

        return Thing.Create($"{dataType}-{treeData.Sid}", dataType, properties, treeData.Lat, treeData.Lng, new List<Observation> {observation});
    }

    public static Thing MapDksrResponse(ParkingSpaceSensorData parkingSpace, string dataType)
    {
        var properties = new Dictionary<string, string>
        {
            { "Id", parkingSpace.Sid }, 
            { "ParkingSpaceId", parkingSpace.ParkingSpaceId.ToString() },
            { "ParkingLotId", parkingSpace.ParkingLotId.ToString() }
        };

        var observation = new Observation
        {
            Name = "Occupancy",
            Result = parkingSpace.Occupied,
            ObservationType = ObservedProperty.Occupancy(),
            PhenomenonTime = parkingSpace.Timestamp,
            ResultTime = parkingSpace.Timestamp
        };

        return Thing.Create($"{dataType}-{parkingSpace.Sid}", dataType, properties, parkingSpace.Lat, parkingSpace.Lng, new List<Observation> {observation});
    }
    
    public static Thing MapDksrResponse(WeatherSensorData weather, string dataType)
    {
        var properties = new Dictionary<string, string>
        {
            { "Id", weather.Sid },
            { "WeatherId", weather.WeatherId.ToString() },
            { "Temperature", weather.Temp.ToString(CultureInfo.InvariantCulture) },
            { "Max temperature", weather.TempMax.ToString(CultureInfo.InvariantCulture) },
            { "Min temperature", weather.TempMin.ToString(CultureInfo.InvariantCulture) },
            { "Pressure", weather.Pressure.ToString() },
            { "Humidity", weather.Humidity.ToString() },
            { "Wind speed", weather.WindSpeed.ToString(CultureInfo.InvariantCulture) },
            { "Wind direction", weather.WindDeg.ToString() },
            { "Cloudiness", weather.Clouds.ToString() }
        };

        List<Observation> observations = new List<Observation> ()
        {
            new () { Name = "Temp", Result = weather.Temp, ObservationType = ObservedProperty.Temperature(), UnitOfMeasurementName = "Degrees", UnitOfMeasurementSymbol = "\u00b0C"},
            new () { Name = "TempMin", Result = weather.TempMin, ObservationType = ObservedProperty.Temperature(), UnitOfMeasurementName = "Degrees", UnitOfMeasurementSymbol = "\u00b0C" },
            new () { Name = "TempMax", Result = weather.TempMax, ObservationType = ObservedProperty.Temperature(), UnitOfMeasurementName = "Degrees", UnitOfMeasurementSymbol = "\u00b0C" },
            new () { Name = "Pressure", Result = weather.Pressure, ObservationType = ObservedProperty.Pressure(), UnitOfMeasurementName = "Pressure", UnitOfMeasurementSymbol = "hPa" },
            new () { Name = "Humidity", Result = weather.Humidity, ObservationType = ObservedProperty.Humidity(), UnitOfMeasurementName = "Percentage", UnitOfMeasurementSymbol = "%" },
            new () { Name = "Sunrise", Result = weather.Sunrise, ObservationType = ObservedProperty.Timestamp(), UnitOfMeasurementName = "Timestamp" },
            new () { Name = "WindSpeed", Result = weather.WindSpeed, ObservationType = ObservedProperty.Speed(), UnitOfMeasurementName = "Speed", UnitOfMeasurementSymbol = "km/h" },
            new () { Name = "WindDeg", Result = weather.WindDeg, ObservationType = ObservedProperty.Direction() },
            new () { Name = "Clouds", Result = weather.Clouds, ObservationType = ObservedProperty.Count(), UnitOfMeasurementName = "Number" },
            new () { Name = "Rain", Result = weather.Rain, ObservationType = ObservedProperty.Count() },
            new () { Name = "Visibility", Result = weather.Visibility, ObservationType = ObservedProperty.Count() },
        };

        foreach (var observation in observations)
        {
            observation.ResultTime = weather.Timestamp;
            observation.PhenomenonTime = weather.Timestamp;
        }

        return Thing.Create(dataType, dataType, properties, weather.Lat, weather.Lng, observations);
    }

    public static DataStream MapFrostResponseToDataStream(DataStreamResponse? response)
    {
        if (response == null)
        {
            return new DataStream();
        }

        var datastream = new DataStream
        {
            Id = response.Id,
            Name = response.Name,
            Description = response.Description,
            ObservationType = response.ObservationType,
            UnitOfMeasurement = response.UnitOfMeasurement,
            ObservedArea = response.ObservedArea,
            Properties = response.Properties
        };

        return datastream;
    }
}