using Newtonsoft.Json;

namespace FrostApi.Models.Observation;

public class Observation
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("result")] public dynamic Result { get; set; } = null!;

    [JsonProperty("phenomenonTime")] public DateTime PhenomenonTime { get; set; }

    [JsonProperty("Datastream")] public Dictionary<string, string> DataStream { get; set; } = null!;

    [JsonIgnore] public Dictionary<string, string> FeatureOfInterest { get; set; } = null!;
}