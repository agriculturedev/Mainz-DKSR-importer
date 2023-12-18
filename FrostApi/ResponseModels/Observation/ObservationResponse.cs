using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Observation;

public class ObservationResponse
{
    [JsonProperty("@iot.selfLink")]
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")]
    public int Id { get; set; }

    public DateTime PhenomenonTime { get; set; }

    public DateTime? ResultTime { get; set; }

    public double Result { get; set; }

    [JsonProperty("Datastream@iot.navigationLink")]
    public string DatastreamNavigationLink { get; set; }
    
    [JsonProperty("FeatureOfInterest@iot.navigationLink")]
    public string FeatureOfInterestNavigationLink { get; set; }
}