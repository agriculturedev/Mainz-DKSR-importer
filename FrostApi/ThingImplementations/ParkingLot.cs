using FrostApi.Models.Thing;
using Newtonsoft.Json;

namespace FrostApi.ThingImplementations;

public class ParkingLot : IThing
{
    [JsonIgnore] public int Id { get; set; }
    
    [JsonProperty("description")] [JsonRequired]
    public const string Description = "ParkingLot";

    [JsonProperty("properties")]
    [JsonRequired]
    public ThingProperties Properties { get; set; }
    
    [JsonProperty("name")] [JsonRequired] public string Name { get; set; }
    [JsonIgnore] public double Lat { get; set; }
    [JsonIgnore] public double Lon { get; set; }
}

