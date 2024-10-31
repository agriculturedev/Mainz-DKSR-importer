using Newtonsoft.Json;

namespace FrostApi.Models.ObservedProperty;

public class ObservedProperty
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("description")] public string Description { get; set; } = null!;

    [JsonProperty("definition")] public string Definition { get; set; } = null!;

    [JsonProperty("properties")] public Dictionary<string, string> Properties { get; set; } = null!;

    public static ObservedProperty Unknown()
    {
        return new ObservedProperty
        {
            Name = "Unknown",
            Description = "Unknown observed property",
            Definition = "Unknown",
        };
    }

    public static ObservedProperty HealthState()
    {
        return new ObservedProperty
        {
            Name = "HealthState",
            Description = "HealthState of a tree",
            Definition = "HealthState",
        };
    }

    public static ObservedProperty Occupancy()
    {
        return new ObservedProperty
        {
            Name = "Occupancy",
            Description = "Occupancy of a parking space",
            Definition = "Occupancy",
        };
    }

    public static ObservedProperty Temperature()
    {
        return new ObservedProperty
        {
            Name = "Temperature",
            Description = "Temperature of an observation",
            Definition = "Temperature",
        };
    }

    public static ObservedProperty Pressure()
    {
        return new ObservedProperty
        {
            Name = "Pressure",
            Description = "Pressure of an observation",
            Definition = "Pressure",
        };
    }

    public static ObservedProperty Speed()
    {
        return new ObservedProperty
        {
            Name = "Speed",
            Description = "Speed of an observation",
            Definition = "Speed",
        };
    }

    public static ObservedProperty Humidity()
    {
        return new ObservedProperty
        {
            Name = "Humidity",
            Description = "Humidity of an observation",
            Definition = "Humidity",
        };
    }

    public static ObservedProperty Timestamp()
    {
        return new ObservedProperty
        {
            Name = "Timestamp",
            Description = "Timestamp of an observation",
            Definition = "Timestamp",
        };
    }

    public static ObservedProperty Direction()
    {
        return new ObservedProperty
        {
            Name = "Direction",
            Description = "Direction of an observation",
            Definition = "Direction",
        };
    }

    public static ObservedProperty Count()
    {
        return new ObservedProperty
        {
            Name = "Count",
            Description = "Count of an observation",
            Definition = "Count",
        };
    }
}