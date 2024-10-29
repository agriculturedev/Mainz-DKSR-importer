using Newtonsoft.Json;

namespace FrostApi.ResponseModels.DataStream;

public class GetDataStreamsResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }

    [JsonProperty("value")] public List<DataStreamResponse?> Value { get; set; } = null!;
}