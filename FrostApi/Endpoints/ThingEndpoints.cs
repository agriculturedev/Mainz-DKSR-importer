using FrostApi.ResponseModels.Thing;
using IThing = FrostApi.Models.Thing.IThing;

namespace FrostApi.Endpoints;

public class ThingEndpoints : FrostHttpClient
{
    public ThingEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    {
    }

    public async Task<GetThingsResponse> GetAllThings(string? filter = null)
    {
        var url = Endpoints.GetEndpointUrl(Endpoints.Things);
        if (filter != null) url += filter;
        var response = await Client.GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetThingsResponse>();
        return result;
    }

    public async Task<HttpResponseMessage> PostThing(IThing thing)
    {
        var content = CreateJsonContent(thing);
        var response = await Client.PostAsync(Endpoints.GetEndpointUrl(Endpoints.Things), content);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateThing(IThing thing)
    {
        var content = CreateJsonContent(thing);
        var url = $"{Endpoints.GetEndpointUrl(Endpoints.Things)}({thing.Id})";
        var response = await Client.PatchAsync(url, content);
        return response;
    }
}