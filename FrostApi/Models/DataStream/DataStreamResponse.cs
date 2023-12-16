using Newtonsoft.Json;

namespace FrostApi.ResponseModels.DataStreams;

public class DataStreamResponse
{
    [JsonProperty("@iot.selfLink")] 
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")] 
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ObservationType { get; set; }

    public UnitOfMeasurement UnitOfMeasurement { get; set; }
    
    public ObservedArea ObservedArea { get; set; }
    
    public DateTime PhenomenonTime { get; set; }

    [JsonProperty("ObservedProperty@iot.navigationLink")]
    public string ObservedPropertyNavigationLink { get; set; } 
    
    [JsonProperty("Sensor@iot.navigationLink")]
    public string SensorNavigationLink { get; set; } 
    
    [JsonProperty("Thing@iot.navigationLink")]
    public string ThingNavigationLink { get; set; } 
    
    [JsonProperty("Observations@iot.navigationLink")]
    public string ObservationsNavigationLink { get; set; }
    
}