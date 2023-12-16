using Newtonsoft.Json;

namespace FrostApi.ResponseModels.DataStreams;

public class GetAllDataStreamsResponse
{
    [JsonProperty("@iot.count")] 
    public int Count { get; set; }

    [JsonProperty("value")] 
    public List<DataStreamResponse> Value { get; set; }
}