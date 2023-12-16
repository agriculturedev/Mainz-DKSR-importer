using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Sensors;

public class SensorResponse
{
    [JsonProperty("@iot.selfLink")] 
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")] 
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
    
    public string EncodingType { get; set; }
    
    public string MetaData { get; set; }

    public Dictionary<string, string> Properties { get; set; }
    
    [JsonProperty("Datastreams@iot.navigationLink")] 
    public string DatastreamsNavigationLink { get; set; }
}