using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Locations;

public class GetAllLocationsResponse
{
    [JsonProperty("@iot.count")] 
    public int Count { get; set; }

    [JsonProperty("value")] 
    public List<LocationResponse> Value { get; set; }
}