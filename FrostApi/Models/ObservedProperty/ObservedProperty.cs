using Newtonsoft.Json;

namespace FrostApi.Models.ObservedProperty;

public class ObservedProperty
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("description")] public string Description { get; set; } = null!;

    [JsonProperty("definition")] public string Definition { get; set; } = null!;

    [JsonProperty("properties")] public Dictionary<string, string> Properties { get; set; } = null!;
}