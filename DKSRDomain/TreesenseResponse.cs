using Newtonsoft.Json;

namespace DKSRDomain;

public class TreesenseResponse
{
    public List<TreeSenseSensorData> SensorData { get; set; } = null!;
}

// {
//   "health_state": 3,
//   "lng": "8.2155556000000000000000",
//   "name": "Eberesche 1",
//   "id": "131",
//   "hardware_serials": [
//     "A8404136C183FB70"
//   ],
//   "lat": "50.0081545000000000000000",
//   "timestamp": "2023-12-15T10:38:09.854Z",
//   "SID": "1098f306-cbcc-4c53-8893-0bf9b999ba13",
//   "_headers": {
//     "eventType": "MainzTreeSenseEventType"
//   }
// },

public class TreeSenseSensorData
{
    [JsonProperty("health_state")] public int HealthState { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("id")] public string Id { get; set; } = null!;

    [JsonProperty("hardware_serials")] public string[] HardwareSerials { get; set; } = null!;

    [JsonProperty("lat")] public string Lat { get; set; } = null!;

    [JsonProperty("lng")] public string Lng { get; set; } = null!;

    [JsonProperty("timestamp")] public DateTime Timestamp { get; set; }

    [JsonProperty("SID")] public string Sid { get; set; } = null!;

    [JsonProperty("_headers")] public Dictionary<string, string> Headers { get; set; } = null!;
}