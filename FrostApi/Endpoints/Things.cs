using System.Net.Mime;
using System.Text;
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
        var response = await _client.GetAsync(_endpoints.GetEndpointUrl(_endpoints.Things));
        var result = await response.Content.ReadAsAsync<GetAllThingsResponse>();
        return result;
    }
    
    public async Task<HttpResponseMessage> PostThing(IThing thing)
    {
        var jsonContent = JsonConvert.SerializeObject(thing);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.PostAsync(_endpoints.GetEndpointUrl(_endpoints.Things), content);
        return response;
    }
    
    public async Task<HttpResponseMessage> UpdateThing(IThing thing)
    {
        var jsonContent = JsonConvert.SerializeObject(thing);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        var url = _endpoints.GetEndpointUrl(_endpoints.Things) + "(" + thing.Id + ")";
        var response = await _client.PatchAsync(url, content);
        return response;
    }
}