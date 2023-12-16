using Newtonsoft.Json;

namespace FrostApi.ResponseModels;

public class Thing
{
    [JsonProperty("name")] 
    public string Name { get; set; }
    
    [JsonProperty("description")] 
    public string Description { get; set; }
    

    // public Dictionary<string, string> Properties { get; set; }
}