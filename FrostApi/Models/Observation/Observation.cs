using Newtonsoft.Json;

namespace FrostApi.Models.Observation;

public class Observation
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("result")] public dynamic Result { get; set; }

    [JsonProperty("phenomenonTime")] public DateTime PhenomenonTime { get; set; }

    [JsonProperty("Datastream")] public Dictionary<string, string> DataStream { get; set; }

    [JsonIgnore] public Dictionary<string, string> FeatureOfInterest { get; set; }
}