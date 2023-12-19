using Newtonsoft.Json;

namespace FrostApi.Models.DataStream;

public class UnitOfMeasurement
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("symbol")] public string Symbol { get; set; }

    [JsonProperty("definition")] public string Definition { get; set; }
}