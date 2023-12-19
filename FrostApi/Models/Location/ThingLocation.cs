using Newtonsoft.Json;

namespace FrostApi.Models.Location;

public class ThingLocation
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("encodingType")] public string EncodingType { get; set; }

    [JsonProperty("location")] public LocationProperties Location { get; set; }

    public IList<Dictionary<string, string>> Things { get; set; }
}