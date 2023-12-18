namespace FrostApi.Models.Location;

public class ThingLocation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string EncodingType { get; set; }
    public LocationProperties Location { get; set; }
    public IList<string> ThingIds { get; set; }
}