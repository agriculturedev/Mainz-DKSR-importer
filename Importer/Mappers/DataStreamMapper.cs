using FrostApi.Models.DataStream;
using FrostApi.ResponseModels.DataStream;

namespace Importer.Mappers;

public static class DataStreamMapper
{
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