using Newtonsoft.Json;

namespace FrostApi.Models.Thing;

public class Thing
{
    [JsonIgnore] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; private set; } = null!;
    [JsonProperty("description")] public string Description { get; private set; } = null!;

    [JsonProperty("properties")]
    [JsonRequired]
    public Dictionary<string, string> Properties { get; private set; } = null!;

    [JsonIgnore] public double Lat { get; private set; }
    [JsonIgnore] public double Lon { get; private set; }
    [JsonIgnore] public List<Observation.Observation> LatestObservations { get; private set; } = null!;

    public static Thing Create(string name, string description, Dictionary<string, string> properties, double lat,
        double lon, List<Observation.Observation> latestObservations)
    {
        if (!properties.ContainsKey("Id"))
            throw new ArgumentException("Properties must contain a key 'Id'");

        return new Thing
        {
            Name = name,
            Description = description,
            Properties = properties,
            Lat = lat,
            Lon = lon,
            LatestObservations = latestObservations
        };
    }
}