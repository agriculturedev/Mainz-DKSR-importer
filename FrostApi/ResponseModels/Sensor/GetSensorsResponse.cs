using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Sensor;

public class GetSensorsResponse
{
    [JsonProperty("@iot.count")] public int Count { get; set; }

    [JsonProperty("value")] public List<SensorResponse> Value { get; set; }
}