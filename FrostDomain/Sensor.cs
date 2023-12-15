namespace FrostDomain;

public class Sensor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public String Description { get; set; }
    public SensorProperties Properties { get; set; }
    public string EncodingType { get; set; }
    public dynamic Metadata { get; set; }
}


// SensorProperties can be one of the following:
// - TreeProperties
// - ParkingLotProperties

public class SensorProperties
{
    public string Type { get; set; }
    public double[] Coordinates { get; set; } // first lng then lat
}