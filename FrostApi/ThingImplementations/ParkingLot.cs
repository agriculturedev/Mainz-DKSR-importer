using FrostApi.Models.Observation;
using FrostApi.Models.Thing;
using Newtonsoft.Json;

namespace FrostApi.ThingImplementations;

public class ParkingLot : IThing
{
    [JsonProperty("description")] [JsonRequired]
    public const string Description = "ParkingLot";

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("properties")]
    [JsonRequired]
    public IThingProperties Properties { get; set; }

    [JsonProperty("name")] [JsonRequired] public string Name { get; set; }
    [JsonIgnore] public double Lat { get; set; }
    [JsonIgnore] public double Lon { get; set; }
    [JsonIgnore] public Observation LatestObservation { get; set; }
}

public class ParkingLotProperties : IThingProperties
{
    [JsonProperty("parkingLotId")]
    [JsonRequired]
    public int ParkingLotId { get; set; }

    [JsonProperty("id")] [JsonRequired] public int Id { get; set; }
}