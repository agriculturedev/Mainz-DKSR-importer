using FrostApi.Models.Observation;
using FrostApi.ResponseModels.Observation;

namespace FrostApi.Endpoints;

public class ObservationEndpoints : FrostHttpClient
{
    public ObservationEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    {
    }

    public async Task<GetObservationsResponse> GetAllDataStreams()
    {
        var response = await GetAsync(Endpoints.GetEndpointUrl(Endpoints.Observations));
        var result = await response.Content.ReadAsAsync<GetObservationsResponse>();
        return result;
    }

    public async Task<GetObservationsResponse> GetObservationsForDataStream(int id)
    {
        var response =
            await GetAsync(Endpoints.GetEndpointForEntityUrl(Endpoints.DataStreams, Endpoints.Observations, id));
        var result = await response.Content.ReadAsAsync<GetObservationsResponse>();
        return result;
    }

    public async Task<HttpResponseMessage> PostObservation(Observation observation)
    {
        var content = CreateJsonContent(observation);
        var response = await PostAsync(Endpoints.GetEndpointUrl(Endpoints.Observations), content);
        return response;
    }
}