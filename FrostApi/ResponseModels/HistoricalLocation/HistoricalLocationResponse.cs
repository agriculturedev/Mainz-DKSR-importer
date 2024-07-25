using Newtonsoft.Json;

namespace FrostApi.ResponseModels.HistoricalLocation;

public class HistoricalLocationResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Time { get; set; } = null!;

    [JsonProperty("Thing@iot.navigationLink")]
    public string ThingNavigationLink { get; set; } = null!;

    [JsonProperty("Locations@iot.navigationLink")]
    public string LocationsNavigationLink { get; set; } = null!;
}