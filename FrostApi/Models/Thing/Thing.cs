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
    [JsonIgnore] public Observation.Observation LatestObservation { get; private set; } = null!;

    public static Thing Create(string name, string description, Dictionary<string, string> properties, double lat,
        double lon, Observation.Observation latestObservation)
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
            LatestObservation = latestObservation
        };
    }

    public static Thing Create(string name, string description, Dictionary<string, string> properties, string lat,
        string lon, Observation.Observation latestObservation)
    {
        return Create(name, description, properties, double.Parse(lat), double.Parse(lon), latestObservation);
    }
}