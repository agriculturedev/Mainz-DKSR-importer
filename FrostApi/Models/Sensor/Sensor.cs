using Newtonsoft.Json;

namespace FrostApi.Models.Sensor;

public class Sensor
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("encodingType")] public string EncodingType { get; set; }

    [JsonProperty("properties")] public SensorProps Properties { get; set; }

    [JsonProperty("metadata")] public string MetaData { get; set; }
}

public class SensorProps
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}