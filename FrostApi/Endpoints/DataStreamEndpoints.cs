using FrostApi.Models.DataStream;
using FrostApi.ResponseModels.DataStream;

namespace FrostApi.Endpoints;

public class DataStreamEndpoints : FrostHttpClient
{
    public DataStreamEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    {
    }

    public async Task<GetDataStreamsResponse> GetAllDataStreams()
    {
        var response = await GetAsync(Endpoints.GetEndpointUrl(Endpoints.DataStreams));
        var result = await response.Content.ReadAsAsync<GetDataStreamsResponse>();
        return result;
    }

    public async Task<GetDataStreamsResponse?> GetDataSteamsForThing(int id)
    {
        var url = Endpoints.GetEndpointForEntityUrl(Endpoints.Things, Endpoints.DataStreams, id);
        var response = await GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetDataStreamsResponse>();
        return result;
    }

    public async Task<HttpResponseMessage> PostDataStream(DataStream dataStream)
    {
        var content = CreateJsonContent(dataStream);
        var response = await PostAsync(Endpoints.GetEndpointUrl(Endpoints.DataStreams), content);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateDataStream(DataStream dataStream)
    {
        var content = CreateJsonContent(dataStream);
        var url = Endpoints.GetEndpointUrl(Endpoints.DataStreams) + "(" + dataStream.Id + ")";
        var response = await PatchAsync(url, content);
        return response;
    }
}