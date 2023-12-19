using FrostApi.Models.Thing;
using Newtonsoft.Json;

namespace FrostApi.ThingImplementations;

public class Tree : IThing
{
    [JsonProperty("description")] [JsonRequired]
    public const string Description = "Tree";

    [JsonProperty("properties")]
    [JsonRequired]
    public TreeProps Properties { get; set; }

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("name")] [JsonRequired] public string Name { get; set; }
}

public class TreeProps
{
    [JsonProperty("id")] [JsonRequired] public int Id { get; set; }
}