using Newtonsoft.Json;

namespace FrostApi.ResponseModels.ObservedProperty;

public class ObservedPropertyResponse
{
    [JsonProperty("@iot.selfLink")] 
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")] 
    public int Id { get; set; }

    public string Name { get; set; }

    public string Definition { get; set; }

    public string Description { get; set; }

    [JsonProperty("Datastreams@iot.navigationLink")]
    public string DatastreamsNavigationLink { get; set; }
}