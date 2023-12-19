using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class Importer
{
    protected readonly HttpClient Client;
    protected readonly FrostApi.FrostApi FrostApi;
    protected readonly ILogger Logger;

    protected Importer(ILogger logger, IConfiguration config)
    {
        FrostApi = new FrostApi.FrostApi(config["FrostBaseUrl"] ?? throw new ArgumentNullException("FrostApiBaseUrl"));
        Username = config["Authentication:Username"] ?? throw new ArgumentNullException("Username");
        Password = config["Authentication:Password"] ?? throw new ArgumentNullException("Username");
        Client = SetupHttpClient();
        Logger = logger;
    }

    protected static string Username { get; set; }
    protected static string Password { get; set; }

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
        client.DefaultRequestHeaders.Add("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}");
        return client;
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
        return await Client!.SendAsync(request);
    }
}