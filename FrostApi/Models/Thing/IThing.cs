using Newtonsoft.Json;

namespace FrostApi.Models.Thing;

public interface IThing
{
    [JsonProperty("description")] public const string Description = "Empty thing";

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
    
    
    [JsonProperty("properties")]
    [JsonRequired]
    public ThingProperties Properties { get; set; }
    
    
    [JsonIgnore] public double Lat { get; set; }
    [JsonIgnore] public double Lon { get; set; }

}

public class ThingProperties
{
    [JsonProperty("id")] [JsonRequired] public int Id { get; set; }
}