using FrostApi.Models.Observation;
using FrostApi.Models.Thing;
using Newtonsoft.Json;

namespace FrostApi.ThingImplementations;

public class Tree : IThing
{
    [JsonProperty("description")] [JsonRequired]
    public const string Description = "Tree";

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("properties")]
    [JsonRequired]
    public IThingProperties Properties { get; set; }

    [JsonProperty("name")] [JsonRequired] public string Name { get; set; }
    [JsonIgnore] public double Lat { get; set; }
    [JsonIgnore] public double Lon { get; set; }
    [JsonIgnore] public Observation LatestObservation { get; set; }
}

public class TreeProperties : IThingProperties
{
    [JsonProperty("id")] [JsonRequired] public int Id { get; set; }
}