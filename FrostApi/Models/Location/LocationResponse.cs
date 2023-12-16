using Newtonsoft.Json;

namespace FrostApi.ResponseModels.Locations;

public class LocationResponse 
{
    [JsonProperty("@iot.selfLink")] 
    public string SelfLink { get; set; }

    [JsonProperty("@iot.id")] 
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string Description { get; set; }
    
    public string EncodingType { get; set; }

    public LocationProperties Location { get; set; }
    
    [JsonProperty("HistoricalLocations@iot.navigationLink")]
    public string HistoricalLocationsNavigationLink { get; set; }
    
    [JsonProperty("Things@iot.navigationLink")]
    public string ThingsNavigationLink { get; set; }

}