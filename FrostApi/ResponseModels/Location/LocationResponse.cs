using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Location;

public class LocationResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string EncodingType { get; set; } = null!;

    public LocationPropertiesResponse Location { get; set; } = null!;

    [JsonProperty("HistoricalLocations@iot.navigationLink")]
    public string HistoricalLocationsNavigationLink { get; set; } = null!;

    [JsonProperty("Things@iot.navigationLink")]
    public string ThingsNavigationLink { get; set; } = null!;
}