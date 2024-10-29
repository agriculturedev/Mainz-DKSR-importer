namespace FrostApi.ResponseModels.Location;

public class LocationPropertiesResponse
{
    public string Type { get; set; } = null!;

    public IList<double> Coordinates { get; set; } = null!;
}