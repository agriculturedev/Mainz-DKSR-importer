namespace Importer.Configuration;

public class DataSources
{
    public List<DataSource> Sources { get; set; } = new();
}

public class DataSource
{
    public string SourceUrl { get; set; } = null!;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string ImporterType { get; set; } = null!;
    public string DestinationUrl { get; set; } = null!;
}