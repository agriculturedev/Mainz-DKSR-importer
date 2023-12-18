using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Thing;

public class ThingResponse
{
    [JsonProperty("@iot.selfLink")]
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Dictionary<string, string> Properties { get; set; }

    [JsonProperty("HistoricalLocations@iot.navigationLink")]
    public string HistoricalLocationsNavigationLink { get; set; }

    [JsonProperty("Locations@iot.navigationLink")]
    public string LocationsNavigationLink { get; set; }

    [JsonProperty("Datastreams@iot.navigationLink")]
    public string DatastreamsNavigationLink { get; set; }
}