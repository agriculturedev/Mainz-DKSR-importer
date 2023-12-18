namespace FrostApi.ResponseModels;

public interface IThing
{
    public int Id { get; set; }
    public string Name { get; set; }
    public const string Description = "Empty thing";
}