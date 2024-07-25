using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Thing;

public class GetThingsResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }
    [JsonProperty("value")] public List<ThingResponse> Value { get; set; } = null!;

    [JsonProperty("@iot.nextLink")] public string NextLink { get; set; } = null!;
}