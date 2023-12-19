namespace FrostApi.ResponseModels.Location;

public class LocationPropertiesResponse
{
    public string Type { get; set; }

    public IList<string> Coordinates { get; set; }
}