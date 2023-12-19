using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.Thing;
using FrostApi.ThingImplementations;

namespace Importer;

public static class Mappers
{
    public static Tree MapDksrResponse(TreeSenseSensorData treeData)
    {
        return new Tree
        {
            Name = treeData.Name,
            Properties = new ThingProperties() { Id = int.Parse(treeData.Id) },
            Lat = double.Parse(treeData.Lat),
            Lon = double.Parse(treeData.Lng)
        };
    }
    
    public static ParkingLot MapDksrResponse(ParkingLotSensorData parkingLot)
    {
        return new ParkingLot
        {
            Name = parkingLot.Sid,
            Properties = new ThingProperties() { Id = parkingLot.ParkingSpaceId },
            Lat = parkingLot.Lat,
            Lon = parkingLot.Lon
        };
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