using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace FrostApi.Endpoints;

public class FrostHttpClient
{
    private HttpClient? _client;
    
    public FrostHttpClient()
    {
        SetupHttpClient();
    }
    
    private void SetupHttpClient()
    {
        if (_client == null)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.All
            };        
            _client = new HttpClient(handler);
        }

        if (!_client.DefaultRequestHeaders.Contains("Accept"))
        {
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        
        if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _client.DefaultRequestHeaders.Add("User-Agent",  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        }
        
        if (!_client.DefaultRequestHeaders.Contains("Accept-Encoding"))
        {
            _client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        }
        
        if (!_client.DefaultRequestHeaders.Contains("Accept-Language"))
        {
            _client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,nl;q=0.8");
        }
    }
    
    public async Task<HttpResponseMessage> PostAsync(string requestUrl, StringContent requestContent)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = requestContent
        };

        return await SendRequest(request);
    }
    
    public async Task<HttpResponseMessage> PatchAsync(string requestUrl, StringContent requestContent)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl)
        {
            Content = requestContent
        };

        return await SendRequest(request);
    }
    
    public async Task<HttpResponseMessage> GetAsync(string requestUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        return await SendRequest(request);
    }
    
    private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
    {
        return await _client!.SendAsync(request);
    }
}