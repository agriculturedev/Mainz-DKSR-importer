using FrostApi.Models.DataStream;
using Newtonsoft.Json;

namespace FrostApi.ResponseModels.DataStream;

public class DataStreamResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ObservationType { get; set; } = null!;

    public UnitOfMeasurement UnitOfMeasurement { get; set; } = null!;

    public ObservedArea ObservedArea { get; set; } = null!;

    public string PhenomenonTime { get; set; } = null!;

    public Dictionary<string, string> Properties { get; set; } = null!;

    [JsonProperty("ObservedProperty@iot.navigationLink")]
    public string ObservedPropertyNavigationLink { get; set; } = null!;

    [JsonProperty("Sensor@iot.navigationLink")]
    public string SensorNavigationLink { get; set; } = null!;

    [JsonProperty("Thing@iot.navigationLink")]
    public string ThingNavigationLink { get; set; } = null!;

    [JsonProperty("Observations@iot.navigationLink")]
    public string ObservationsNavigationLink { get; set; } = null!;
}