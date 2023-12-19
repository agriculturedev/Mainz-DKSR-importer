namespace FrostApi.Constants;

public class Endpoints
{
    public string DataStreams = "/Datastreams";
    public string FeaturesOfInterest = "/FeaturesOfInterest";
    public string HistoricalLocations = "/HistoricalLocations";
    public string Locations = "/Locations";
    public string Observations = "/Observations";
    public string ObservedProperties = "/ObservedProperties";
    public string Sensors = "/Sensors";
    public string Things = "/Things";

    public Endpoints(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; set; }

    public string GetEndpointUrl(string endpoint)
    {
        return $"{BaseUrl}{endpoint}";
    }

    public string GetEndpointForEntityUrl(string entity, string endpoint, int entityId)
    {
        return $"{BaseUrl}{entity}({entityId}){endpoint}";
    }
}