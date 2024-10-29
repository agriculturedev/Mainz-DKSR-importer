using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Observation;

public class ObservationResponse
{
    [JsonProperty("@iot.selfLink")] public string SelfLink { get; set; } = null!;

    [JsonProperty("@iot.id")] public int Id { get; set; }

    public DateTime PhenomenonTime { get; set; }

    public DateTime? ResultTime { get; set; }

    public dynamic Result { get; set; } = null!;

    [JsonProperty("Datastream@iot.navigationLink")]
    public string DatastreamNavigationLink { get; set; } = null!;

    [JsonProperty("FeatureOfInterest@iot.navigationLink")]
    public string FeatureOfInterestNavigationLink { get; set; } = null!;
}