using FrostApi.Models.Sensor;
using FrostApi.ResponseModels.DataStream;
using FrostApi.ResponseModels.Sensor;

namespace FrostApi.Endpoints;

public class SensorEndpoints : FrostHttpClient
{
    public SensorEndpoints(Constants.Endpoints endpoints) : base(endpoints)
    { }
    
    public async Task<GetSensorsResponse> GetAllSensors()
    {
        var response = await GetAsync(Endpoints.GetEndpointUrl(Endpoints.Sensors));
        var result = await response.Content.ReadAsAsync<GetSensorsResponse>();
        return result;
    }
    
    public async Task<GetDataStreamsResponse?> GetSensorsForDataStream(int id)
    {
        var url = Endpoints.GetEndpointForEntityUrl(Endpoints.DataStreams, Endpoints.Sensors, id);
        var response = await GetAsync(url);
        var result = await response.Content.ReadAsAsync<GetDataStreamsResponse>();
        return result;
    }
    
    public async Task<HttpResponseMessage> PostSensor(Sensor Sensor)
    {
        var content = CreateJsonContent(Sensor);
        var response = await PostAsync(Endpoints.GetEndpointUrl(Endpoints.Sensors), content);
        return response;
    }
    
    public async Task<HttpResponseMessage> UpdateSensor(Sensor Sensor)
    {
        var content = CreateJsonContent(Sensor);
        var url = Endpoints.GetEndpointUrl(Endpoints.Sensors) + "(" + Sensor.Id + ")";
        var response = await PatchAsync(url, content);
        return response;
    }
}