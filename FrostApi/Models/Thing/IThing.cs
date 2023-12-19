using Newtonsoft.Json;

namespace FrostApi.Models.Thing;

public interface IThing
{
    [JsonProperty("description")] public const string Description = "Empty thing";

    [JsonIgnore] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}