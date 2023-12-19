using Newtonsoft.Json;

namespace FrostApi.ResponseModels.HistoricalLocation;

public class GetAllHistoricalLocationsResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }

    [JsonProperty("value")] public List<HistoricalLocationResponse> Value { get; set; }

    [JsonProperty("@iot.nextLink")] public string NextLink { get; set; }
}