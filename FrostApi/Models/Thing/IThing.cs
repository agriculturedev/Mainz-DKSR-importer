﻿using Newtonsoft.Json;

namespace FrostApi.Models.Thing;

public interface IThing
{
    [JsonProperty("description")] public const string Description = "Empty thing";

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }


    [JsonProperty("properties")]
    [JsonRequired]
    public IThingProperties Properties { get; set; }


    [JsonIgnore] public double Lat { get; set; }
    [JsonIgnore] public double Lon { get; set; }

    [JsonIgnore] public Observation.Observation LatestObservation { get; set; }
}

public interface IThingProperties
{
    [JsonProperty("id")] [JsonRequired] public int Id { get; set; }
}