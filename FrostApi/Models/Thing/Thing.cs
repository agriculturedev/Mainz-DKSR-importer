using Newtonsoft.Json;

namespace FrostApi.Models.Thing;

public class Thing
{
    [JsonProperty("description")] public const string Description = "Empty thing";

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("properties")]
    [JsonRequired]
    public Dictionary<string, string> Properties { get; set; }


    [JsonIgnore] public double Lat { get; set; }
    [JsonIgnore] public double Lon { get; set; }
    [JsonIgnore] public Observation.Observation LatestObservation { get; set; }
}