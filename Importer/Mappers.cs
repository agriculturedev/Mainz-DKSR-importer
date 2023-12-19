using DKSRDomain;
using FrostApi.Models.DataStream;
using FrostApi.Models.Observation;
using FrostApi.Models.Thing;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ThingImplementations;

namespace Importer;

public static class Mappers
{
    public static Tree MapDksrResponse(TreeSenseSensorData treeData)
    {
        return new Tree
        {
            Name = treeData.Name,
            Properties = new TreeProperties { Id = int.Parse(treeData.Id) },
            Lat = double.Parse(treeData.Lat),
            Lon = double.Parse(treeData.Lng),
            LatestObservation = new Observation { Result = treeData.HealthState, PhenomenonTime = treeData.Timestamp }
        };
    }

    public static ParkingLot MapDksrResponse(ParkingLotSensorData parkingLot)
    {
        return new ParkingLot
        {
            Name = parkingLot.Sid,
            Properties = new ParkingLotProperties { Id = parkingLot.ParkingSpaceId, ParkingLotId = parkingLot.ParkingLotId },
            Lat = parkingLot.Lat,
            Lon = parkingLot.Lon,
            LatestObservation = new Observation { Result = parkingLot.Occupied, PhenomenonTime = parkingLot.Timestamp }
        };
    }

    
    // TODO@JOREN: future improvement maybe?
    // public static Thing MapDksrResponse(TreeSenseSensorData treeData, string? test = null)
    // {
    //     return new Thing
    //     {
    //         Name = treeData.Sid,
    //         Properties = new Dictionary<string, string> { { "Id", treeData.Sid } },
    //         Lat = double.Parse(treeData.Lat),
    //         Lon = double.Parse(treeData.Lng),
    //         LatestObservation = new Observation { Result = treeData.HealthState, PhenomenonTime = treeData.Timestamp }
    //     };
    // }
    //
    // public static Thing MapDksrResponse(ParkingLotSensorData parkingLot, string? test = null)
    // {
    //     return new Thing
    //     {
    //         Name = parkingLot.Sid,
    //         Properties = new Dictionary<string, string>
    //         {
    //             { "Id", parkingLot.Sid }, { "ParkingSpaceId", parkingLot.ParkingSpaceId.ToString() },
    //             { "ParkingLotId", parkingLot.ParkingLotId.ToString() }
    //         },
    //         Lat = parkingLot.Lat,
    //         Lon = parkingLot.Lon,
    //         LatestObservation = new Observation { Result = parkingLot.Occupied, PhenomenonTime = parkingLot.Timestamp }
    //     };
    // }

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