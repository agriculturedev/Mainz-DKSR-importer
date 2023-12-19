using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Location;

public class GetLocationsResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }

    [JsonProperty("value")] public List<LocationResponse> Value { get; set; }
}