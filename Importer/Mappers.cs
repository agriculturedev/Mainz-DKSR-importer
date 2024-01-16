using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Observation;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;

namespace Importer;

public static class Mappers
{
    public static Thing MapDksrResponse(TreeSenseSensorData treeData, string dataType)
    {
        var properties = new Dictionary<string, string> { { "Id", treeData.Sid } };

        var observation = new Observation { Result = treeData.HealthState, PhenomenonTime = treeData.Timestamp };

        return Thing.Create($"{dataType}-{treeData.Sid}", dataType, properties, treeData.Lat, treeData.Lng, observation);
    }

    public static Thing MapDksrResponse(ParkingSpaceSensorData parkingSpace, string dataType)
    {
        var properties = new Dictionary<string, string>
        {
            { "Id", parkingSpace.Sid }, 
            { "ParkingSpaceId", parkingSpace.ParkingSpaceId.ToString() },
            { "ParkingLotId", parkingSpace.ParkingLotId.ToString() }
        };

        var observation = new Observation { Result = parkingSpace.Occupied, PhenomenonTime = parkingSpace.Timestamp };

        return Thing.Create($"{dataType}-{parkingSpace.Sid}", dataType, properties, parkingSpace.Lat, parkingSpace.Lon, observation);
    }

    public static DataStream MapFrostResponseToDataStream(DataStreamResponse? response)
    {
        var datastream = new DataStream
        {
            Id = response.Id,
            Name = response.Name,
            Description = response.Description,
            ObservationType = response.ObservationType,
            UnitOfMeasurement = response.UnitOfMeasurement,
            ObservedArea = response.ObservedArea
        };

        return datastream;
    }
}