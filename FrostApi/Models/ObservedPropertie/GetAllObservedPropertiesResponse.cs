using Newtonsoft.Json;

namespace FrostApi.ResponseModels.ObservedProperties;

public class GetAllObservedPropertiesResponse
{
    [JsonProperty("@iot.count")] 
    public int Count { get; set; }

    [JsonProperty("value")] 
    public List<ObservedPropertyResponse> Value { get; set; }
}