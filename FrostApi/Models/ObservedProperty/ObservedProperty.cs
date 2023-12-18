using Newtonsoft.Json;

namespace FrostApi.Models.ObservedProperty;

public class ObservedProperty
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("definition")]
    public string Definition { get; set; }
    
    [JsonProperty("properties")]
    public Dictionary<string, string> Properties { get; set; }

}