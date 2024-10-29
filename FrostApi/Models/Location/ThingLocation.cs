using Newtonsoft.Json;

namespace FrostApi.Models.Location;

public class ThingLocation
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("description")] public string Description { get; set; } = null!;

    [JsonProperty("encodingType")] public string EncodingType { get; set; } = null!;

    [JsonProperty("location")] public LocationProperties Location { get; set; } = null!;

    public IList<Dictionary<string, string>> Things { get; set; } = null!;
}