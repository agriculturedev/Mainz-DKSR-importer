using Newtonsoft.Json;

namespace DKSRDomain;

public class ParkingLotResponse
{
    public ParkingLotSensorData[] SensorData { get; set; }
}

//   {
//     "ignored": false,
//     "parking_space_id": 157002,
//     "level": 0,
//     "last_change": 1702595247,
//     "occupied_preliminary": false,
//     "lon": 8.263611133612132,
//     "SID": "2cb12820-1cf0-482e-890d-51e1da5982ab",
//     "sensor_id": 157002,
//     "has_display": false,
//     "additional_info": "{}",
//     "reserved": false,
//     "last_contact": 1702644005,
//     "xml_id": 95,
//     "parking_lot_id": 2950,
//     "occupied": false,
//     "lat": 50.010667259817694,
//     "timestamp": "2023-12-15T14:24:19.908Z",
//     "_headers": {
//       "eventType": "ParkinglotSNEventType"
//     }

public class ParkingLotSensorData
{
    [JsonProperty("ignored")]
    public bool Ignored { get; set; }
    
    [JsonProperty("parking_space_id")]
    public int ParkingSpaceId { get; set; }
    
    [JsonProperty("level")]
    public int Level { get; set; }
    
    [JsonProperty("last_change")]
    public int LastChange { get; set; }
    
    [JsonProperty("occupied_preliminary")]
    public bool OccupiedPreliminary { get; set; }
    
    [JsonProperty("lon")]
    public double Lon { get; set; }
    
    [JsonProperty("SID")]
    public string Sid { get; set; }
    
    [JsonProperty("sensor_id")]
    public int SensorId { get; set; }
    
    [JsonProperty("had_display")]
    public bool HasDisplay { get; set; }
    
    [JsonProperty("additional_info")]
    public string AdditionalInfo { get; set; }
    
    [JsonProperty("reserved")]
    public bool Reserved { get; set; }
    
    [JsonProperty("last_contact")]
    public int LastContact { get; set; }
    
    [JsonProperty("xml_id")]
    public int XmlId { get; set; }
    
    [JsonProperty("parking_lot_id")]
    public int ParkingLotId { get; set; }
    
    [JsonProperty("occupied")]
    public bool Occupied { get; set; }
    
    [JsonProperty("lat")]
    public double Lat { get; set; }
    
    [JsonProperty("timestamp")]

    public DateTime Timestamp { get; set; }
    
    [JsonProperty("_headers")]

    public Dictionary<string, string> Headers { get; set; }
    
}