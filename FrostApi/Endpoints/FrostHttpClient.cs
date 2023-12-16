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
    
    public HttpRequestMessage CreateGetRequest(string path)
    {
        return new HttpRequestMessage {
            Method = HttpMethod.Get,
            RequestUri = new Uri(path),
            Version = new Version(2, 0) // has to be 1.1 for some older api's, here 2.0 worked 
            
        };
    }
    
    public HttpRequestMessage CreatePostRequest(string path, string bodyContent)
    {
        var httpContent = new StringContent(bodyContent, Encoding.UTF8, "application/json");
        
        return new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri(path),
            Version = new Version(2, 0), // has to be 1.1 for some older api's, here 2.0 worked 
            Content = httpContent
        };
    }
    
    public async Task<T> ExecuteWithTryCatch<T>(Func<HttpRequestMessage> createRequest, Func<HttpResponseMessage, Task<T>> processResponse) where T : new()
    {
        T result = new();
        HttpRequestMessage request = createRequest();

        try
        {
            HttpResponseMessage response = await _client!.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                result = await processResponse(response);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic errorObject = JsonConvert.DeserializeObject(responseContent);
                Console.WriteLine($"Error: {errorObject}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return result;
    }
}