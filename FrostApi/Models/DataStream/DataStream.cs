using Newtonsoft.Json;

namespace FrostApi.Models.DataStream;

public class DataStream
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("observationType")]
    public string ObservationType { get; set; }
    
    [JsonProperty("unitOfMeasurement")]
    public UnitOfMeasurement UnitOfMeasurement { get; set; }
    
    [JsonProperty("observedArea")]
    public ObservedArea ObservedArea { get; set; }
    
    public Dictionary<string, string> Thing { get; set; }
    
    public Dictionary<string, string> Sensor { get; set; }
    
    public Dictionary<string, string>  ObservedProperty { get; set; }
    
}