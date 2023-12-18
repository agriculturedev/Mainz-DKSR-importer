namespace DKSRDomain;

public class TreesenseResponse
{
    public List<TreeSenseSensorData> SensorData { get; set; }
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
    public int HealthState { get; set; }
    public string Lng { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }
    public string[] HardwareSerials { get; set; }
    public string Lat { get; set; }
    public DateTime Timestamp { get; set; }
    public string Sid { get; set; }
    public string[] Headers { get; set; }
}

