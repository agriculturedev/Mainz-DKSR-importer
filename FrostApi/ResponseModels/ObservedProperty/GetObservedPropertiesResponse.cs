using Newtonsoft.Json;

namespace FrostApi.ResponseModels.ObservedProperty;

public class GetObservedPropertiesResponse
{
    [JsonProperty("@iot.count")] 
    public int Count { get; set; }

    [JsonProperty("value")] 
    public List<ObservedPropertyResponse> Value { get; set; }
}