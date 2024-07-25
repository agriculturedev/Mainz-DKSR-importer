using Newtonsoft.Json;

namespace FrostApi.ResponseModels.ObservedProperty;

public class ObservedPropertyResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Definition { get; set; } = null!;

    public string Description { get; set; } = null!;

    [JsonProperty("Datastreams@iot.navigationLink")]
    public string DatastreamsNavigationLink { get; set; } = null!;
}