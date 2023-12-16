using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Observations;

public class GetAllObservationsResponse
{
    [JsonProperty("@iot.count")]
    public int Count { get; set; }

    [JsonProperty("value")]
    public List<ObservationResponse> Value { get; set; }
    
    [JsonProperty("@iot.nextLink")]
    public string NextLink { get; set; }
}