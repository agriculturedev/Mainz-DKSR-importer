using System.Text.Json.Serialization;

namespace DKSRDomain;

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

public class TreeSenseSensorData : IDksrResponse
{
    [JsonPropertyName("health_state")] public int HealthState { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = null!;

    [JsonPropertyName("id")] public string Id { get; set; } = null!;

    [JsonPropertyName("hardware_serials")] public string[] HardwareSerials { get; set; } = null!;


    [JsonPropertyName("SID")] public string Sid { get; set; } = null!;
    [JsonPropertyName("lat")] public double Lat { get; set; }
    [JsonPropertyName("lng")] public double Lng { get; set; }
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("_headers")] public Dictionary<string, string> Headers { get; set; } = null!;
}