using FrostApi.ResponseModels.Location;
using Newtonsoft.Json;

namespace FrostApi.ResponseModels.FeaturesOfInterest;

public class FeaturesOfInterestResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string EncodingType { get; set; } = null!;

    public LocationPropertiesResponse Feature { get; set; } = null!;

    [JsonProperty("Observations@iot.navigationLink")]
    public string ObservationsNavigationLink { get; set; } = null!;
}