using Newtonsoft.Json;

namespace FrostApi.ResponseModels.HistoricalLocation;

public class HistoricalLocationResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; }

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Time { get; set; }

    [JsonProperty("Thing@iot.navigationLink")]
    public string ThingNavigationLink { get; set; }

    [JsonProperty("Locations@iot.navigationLink")]
    public string LocationsNavigationLink { get; set; }
}