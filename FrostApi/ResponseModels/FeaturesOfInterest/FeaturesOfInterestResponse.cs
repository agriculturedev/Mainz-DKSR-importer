using FrostApi.ResponseModels.Location;
using Newtonsoft.Json;

namespace FrostApi.ResponseModels.FeaturesOfInterest;

public class FeaturesOfInterestResponse
{
    [JsonProperty("@iot.selfLink")] 
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")] 
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string EncodingType { get; set; }

    public LocationPropertiesResponse Feature { get; set; }

    [JsonProperty("Observations@iot.navigationLink")]
    public string ObservationsNavigationLink { get; set; }
}