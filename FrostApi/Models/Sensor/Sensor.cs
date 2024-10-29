using Newtonsoft.Json;

namespace FrostApi.Models.Sensor;

public class Sensor
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("description")] public string Description { get; set; } = null!;

    [JsonProperty("encodingType")] public string EncodingType { get; set; } = null!;

    [JsonProperty("properties")] public SensorProps Properties { get; set; } = null!;

    [JsonProperty("metadata")] public string MetaData { get; set; } = null!;
}

public class SensorProps
{
    [JsonProperty("id")] public string Id { get; set; } = null!;

    [JsonProperty("name")] public string Name { get; set; } = null!;
}