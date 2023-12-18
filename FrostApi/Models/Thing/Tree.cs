using Newtonsoft.Json;

namespace FrostApi.ResponseModels;

public class Tree : IThing
{
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonProperty("name")] [JsonRequired]
    public string Name { get; set; }

    [JsonProperty("description")] [JsonRequired]
    public const string Description = "Treesense tree";
    
    public TreeProps Properties { get; set; }
}

public class TreeProps
{
    public int Id { get; set; }
}