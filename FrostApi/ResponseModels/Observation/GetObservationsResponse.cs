using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Observation;

public class GetObservationsResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }

    [JsonProperty("value")] public List<ObservationResponse> Value { get; set; } = null!;

    [JsonProperty("@iot.nextLink")] public string NextLink { get; set; } = null!;
}