using FrostApi.Models.ObservedProperty;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.ObservedProperty;

namespace FrostApi.Endpoints;

public class ObservedPropertyEndpoints : FrostHttpClient
{
    public ObservedPropertyEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    {
    }

    public async Task<GetObservedPropertiesResponse> GetAllObservedProperties()
    {
        var response = await GetAsync(Endpoints.GetEndpointUrl(Endpoints.ObservedProperties));
        var result = await response.Content.ReadAsAsync<GetObservedPropertiesResponse>();
        return result;
    }

    public async Task<GetDataStreamsResponse?> GetObservedPropertiesForDataStream(int id)
    {
        var url = Endpoints.GetEndpointForEntityUrl(Endpoints.DataStreams, Endpoints.ObservedProperties, id);
        var response = await GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetDataStreamsResponse>();
        return result;
    }

    public async Task<HttpResponseMessage> PostObservedProperty(ObservedProperty observedProperty)
    {
        var content = CreateJsonContent(observedProperty);
        var response = await PostAsync(Endpoints.GetEndpointUrl(Endpoints.ObservedProperties), content);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateObservedProperty(ObservedProperty observedProperty)
    {
        var content = CreateJsonContent(observedProperty);
        var url = Endpoints.GetEndpointUrl(Endpoints.ObservedProperties) + "(" + observedProperty.Id + ")";
        var response = await PatchAsync(url, content);
        return response;
    }
}