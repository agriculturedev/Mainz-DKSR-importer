using Newtonsoft.Json;

namespace FrostApi.ResponseModels.FeaturesOfInterest;

public class GetFeaturesOfInterestResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }

    [JsonProperty("value")] public List<FeaturesOfInterestResponse> Value { get; set; } = null!;
}