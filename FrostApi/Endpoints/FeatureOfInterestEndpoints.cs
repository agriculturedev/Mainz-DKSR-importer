using FrostApi.Models.FeatureOfInterest;
using FrostApi.ResponseModels.FeaturesOfInterest;

namespace FrostApi.Endpoints;

public class FeatureOfInterestEndpoints : FrostHttpClient
{
    public FeatureOfInterestEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    {
    }

    public async Task<GetFeaturesOfInterestResponse> GetAllFeaturesOfInterest(string? filter = null)
    {
        var url = Endpoints.GetEndpointUrl(Endpoints.FeaturesOfInterest);
        if (filter != null) url += filter;
        var response = await Client.GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetFeaturesOfInterestResponse>();
        return result;
    }

    public async Task<HttpResponseMessage> PostFeatureOfInterest(FeatureOfInterest FeatureOfInterest)
    {
        var content = CreateJsonContent(FeatureOfInterest);
        var response = await Client.PostAsync(Endpoints.GetEndpointUrl(Endpoints.FeaturesOfInterest), content);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateFeatureOfInterest(FeatureOfInterest FeatureOfInterest)
    {
        var content = CreateJsonContent(FeatureOfInterest);
        var url = $"{Endpoints.GetEndpointUrl(Endpoints.FeaturesOfInterest)}({FeatureOfInterest.Id})";
        var response = await Client.PatchAsync(url, content);
        return response;
    }
}