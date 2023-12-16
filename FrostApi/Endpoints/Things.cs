using FrostApi.ResponseModels;
using Newtonsoft.Json;

namespace FrostApi.Endpoints.Things;

public class Things
{
    private FrostHttpClient _client;
    private Constants.Endpoints _endpoints;
    
    public Things(FrostHttpClient client, Constants.Endpoints endpoints)
    {
        _client = client;
        _endpoints = endpoints;
    }
    
    public async Task<GetAllThingsResponse> GetAllThings()
    {
        return await _client.ExecuteWithTryCatch<GetAllThingsResponse>(
            () => _client.CreateGetRequest(_endpoints.GetEndpointUrl(_endpoints.Things)),
            async response => await response.Content.ReadAsAsync<GetAllThingsResponse>()
        );
    }
    
    public async Task PostThing(Thing thing)
    {
        await _client.ExecuteWithTryCatch(
            () => _client.CreatePostRequest(_endpoints.GetEndpointUrl(_endpoints.Things), JsonConvert.SerializeObject(thing)),
            async response => await response.Content.ReadAsAsync<GetAllThingsResponse>()
        );
    }
}