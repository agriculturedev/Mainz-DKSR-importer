using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Thing;

public class ThingResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Dictionary<string, string> Properties { get; set; } = null!;

    [JsonProperty("HistoricalLocations@iot.navigationLink")]
    public string HistoricalLocationsNavigationLink { get; set; } = null!;

    [JsonProperty("Locations@iot.navigationLink")]
    public string LocationsNavigationLink { get; set; } = null!;

    [JsonProperty("Datastreams@iot.navigationLink")]
    public string DatastreamsNavigationLink { get; set; } = null!;
}