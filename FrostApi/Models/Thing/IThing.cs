using Newtonsoft.Json;

namespace FrostApi.Models.Thing;

public interface IThing
{
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    public const string Description = "Empty thing";
}