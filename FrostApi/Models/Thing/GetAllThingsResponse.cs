using Newtonsoft.Json;

namespace FrostApi.ResponseModels;
public class GetAllThingsResponse
{
    [JsonProperty("@iot.count")]
    public int Count { get; set; }

    [JsonProperty("value")]
    public List<ThingResponse> Value { get; set; }

    [JsonProperty("@iot.nextLink")]
    public string NextLink { get; set; }
}