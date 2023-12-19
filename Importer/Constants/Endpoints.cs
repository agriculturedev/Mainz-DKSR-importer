namespace Importer.Constants;

public static class Endpoints
{
    private const string Prefix = "https://";
    private const string Url = "mainz-staging.dksr.city";

    public const string TreesenseEndpoint =
        "/UrbanPulseData/historic/sensordata?eventtype=fac3edb2-53fa-4319-a897-e3e3c02102cc";

    public const string ParkingLotEndpoint =
        "/UrbanPulseData/historic/sensordata?eventtype=2a4ce3e9-92db-455e-bece-0176c62fafba";

    public static string GetAuthenticatedEndpointUrl(string? username, string? password, string socket)
    {
        // if username or password is null, return the unauthenticated endpoint
        if (username == null || password == null)
            throw new ArgumentException("username or password undefined");
        
        return $"{Prefix}{username}:{password}@{Url}{socket}";
    }

    public static string GetEndpointUrl(string socket)
    {
        return $"{Prefix}{Url}{socket}";
    }
}