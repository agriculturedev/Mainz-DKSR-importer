using Newtonsoft.Json;

namespace FrostApi.Models.DataStream;

public class UnitOfMeasurement
{
    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("symbol")] public string Symbol { get; set; } = null!;

    [JsonProperty("definition")] public string Definition { get; set; } = null!;
}