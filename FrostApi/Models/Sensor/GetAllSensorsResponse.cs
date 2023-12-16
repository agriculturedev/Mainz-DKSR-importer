using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Sensors;

public class GetAllSensorsResponse
{
    [JsonProperty("@iot.count")] 
    public int Count { get; set; }

    [JsonProperty("value")] 
    public List<SensorResponse> Value { get; set; }

}