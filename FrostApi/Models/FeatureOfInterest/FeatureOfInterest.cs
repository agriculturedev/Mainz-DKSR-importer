using FrostApi.Models.Location;
using Newtonsoft.Json;

namespace FrostApi.Models.FeatureOfInterest;

public class FeatureOfInterest
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("encodingType")] public string EncodingType { get; set; }

    [JsonProperty("feature")] public LocationProperties Feature { get; set; }
}