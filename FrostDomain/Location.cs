namespace FrostDomain;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public LocationProperties Properties { get; set; }
    public string EncodingType { get; set; }
    
}

public class LocationProperties
{
    public string Type { get; set; }
    public double[] Coordinates { get; set; } // first lng then lat
}
