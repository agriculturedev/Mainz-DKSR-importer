using Newtonsoft.Json;

namespace FrostApi.Models.DataStream;

public class DataStream
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("description")] public string Description { get; set; } = null!;

    [JsonProperty("observationType")] public string ObservationType { get; set; } = null!;

    [JsonProperty("unitOfMeasurement")] public UnitOfMeasurement UnitOfMeasurement { get; set; } = null!;

    [JsonProperty("observedArea")] public ObservedArea ObservedArea { get; set; } = null!;

    [JsonProperty("properties")] public Dictionary<string, string> Properties { get; set; } = null!;
    public Dictionary<string, string> Thing { get; set; } = null!;

    public Dictionary<string, string> Sensor { get; set; } = null!;

    public Dictionary<string, string> ObservedProperty { get; set; } = null!;
}