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
    public bool Ignored { get; set; }
    public int ParkingSpaceId { get; set; }
    public int Level { get; set; }
    public int LastChange { get; set; }
    public bool OccupiedPreliminary { get; set; }
    public double Lon { get; set; }
    public string Sid { get; set; }
    public int SensorId { get; set; }
    public bool HasDisplay { get; set; }
    public string AdditionalInfo { get; set; }
    public bool Reserved { get; set; }
    public int LastContact { get; set; }
    public int XmlId { get; set; }
    public int ParkingLotId { get; set; }
    public bool Occupied { get; set; }
    public double Lat { get; set; }
    public DateTime Timestamp { get; set; }
    public string[] Headers { get; set; }
}