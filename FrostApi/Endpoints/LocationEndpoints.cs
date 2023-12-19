using FrostApi.Models.Location;
using FrostApi.ResponseModels.Location;

namespace FrostApi.Endpoints;

public class LocationEndpoints : FrostHttpClient
{
    public LocationEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    {
    }

    public async Task<GetLocationsResponse> GetAllLocations(string? filter = null)
    {
        var url = Endpoints.GetEndpointUrl(Endpoints.Locations);
        if (filter != null) url += filter;
        var response = await Client.GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetLocationsResponse>();
        return result;
    }


    public async Task<GetLocationsResponse?> GetLocationsForThing(int id)
    {
        var url = Endpoints.GetEndpointForEntityUrl(Endpoints.Things, Endpoints.Locations, id);
        var response = await GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetLocationsResponse>();
        return result;
    }

    public async Task<HttpResponseMessage> PostLocation(ThingLocation Location)
    {
        var content = CreateJsonContent(Location);
        var response = await Client.PostAsync(Endpoints.GetEndpointUrl(Endpoints.Locations), content);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateLocation(ThingLocation Location)
    {
        var content = CreateJsonContent(Location);
        var url = $"{Endpoints.GetEndpointUrl(Endpoints.Locations)}({Location.Id})";
        var response = await Client.PatchAsync(url, content);
        return response;
    }
}