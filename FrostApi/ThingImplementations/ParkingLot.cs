using FrostApi.Models.Thing;
using Newtonsoft.Json;

namespace FrostApi.ThingImplementations;

public class ParkingLot : IThing
{
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonProperty("name")] [JsonRequired]
    public string Name { get; set; }

    [JsonProperty("description")] [JsonRequired]
    public const string Description = "Tree";
    
    public ParkingLotProps Properties { get; set; }
}

public class ParkingLotProps
{
    public int Id { get; set; }
}