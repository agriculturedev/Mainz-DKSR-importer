namespace DKSRDomain;

public interface IDksrResponse
{
    public string Sid { get; set; }
    public double Lng { get; set; }
    public double Lat { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Headers { get; set; }

}