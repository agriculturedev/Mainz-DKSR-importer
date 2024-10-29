using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Sensor;

public class SensorResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string EncodingType { get; set; } = null!;

    public string MetaData { get; set; } = null!;

    public Dictionary<string, string?> Properties { get; set; } = null!;

    [JsonProperty("Datastreams@iot.navigationLink")]
    public string DatastreamsNavigationLink { get; set; } = null!;
}