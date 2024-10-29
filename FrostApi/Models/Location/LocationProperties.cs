using Newtonsoft.Json;

namespace FrostApi.Models.Location;

public class LocationProperties
{
    [JsonProperty("type")] public string Type { get; set; } = null!;

    [JsonProperty("coordinates")] public IList<double> Coordinates { get; set; } = null!;
}