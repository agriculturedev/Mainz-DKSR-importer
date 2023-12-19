using System.Net;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace FrostApi.Endpoints;

public class FrostHttpClient
{
    protected readonly HttpClient Client;
    protected readonly Constants.Endpoints Endpoints;

    public FrostHttpClient(Constants.Endpoints endpoints)
    {
        Client = SetupHttpClient();
        Endpoints = endpoints;
    }

    private HttpClient SetupHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        };
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,nl;q=0.8");
        return client;
    }

    protected StringContent CreateJsonContent(object obj)
    {
        var jsonContent = JsonConvert.SerializeObject(obj);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        return content;
    }

    protected async Task<HttpResponseMessage> PostAsync(string requestUrl, StringContent requestContent)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
        {
            Content = requestContent
        };

        return await SendRequest(request);
    }

    protected async Task<HttpResponseMessage> PatchAsync(string requestUrl, StringContent requestContent)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl)
        {
            Content = requestContent
        };

        return await SendRequest(request);
    }

    protected async Task<HttpResponseMessage> GetAsync(string requestUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        return await SendRequest(request);
    }

    private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
    {
        return await Client!.SendAsync(request);
    }
}