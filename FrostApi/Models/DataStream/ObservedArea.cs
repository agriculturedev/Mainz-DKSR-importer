using Newtonsoft.Json;

namespace FrostApi.Models.DataStream;

public class ObservedArea
{
    [JsonProperty("type")] public string Type { get; set; } = null!;

    [JsonProperty("coordinates")] public IList<double> Coordinates { get; set; } = null!;
}