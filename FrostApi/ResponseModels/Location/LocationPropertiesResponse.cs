namespace FrostApi.ResponseModels.Location;

public class LocationPropertiesResponse
{
    public string Type { get; set; } = null!;

    public IList<string> Coordinates { get; set; } = null!;
}