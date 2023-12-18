using Newtonsoft.Json;

namespace FrostApi.Models.Observation;

public class Observation
{
    [JsonProperty("id")]

    public int Id { get; set; }

    [JsonProperty("result")]
    public dynamic Result { get; set; }
    [JsonProperty("phenomenonTime")]

    public DateTime PhenomenonTime { get; set; }

    public Dictionary<string, string> DataStreamId { get; set; }
    public Dictionary<string, string> FeatureOfInterestId { get; set; }
}