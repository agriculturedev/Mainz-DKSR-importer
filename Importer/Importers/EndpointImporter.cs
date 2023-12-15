using System.IO.Compression;
using System.Net;
using System.Text;
using DKSRDomain;
using Importer.Constants;

namespace Importer.Importers;

public class EndpointImporter
{
    private HttpClient? _client;
    public static string Username { get; set; }
    public static string Password { get; set; }
    
    public EndpointImporter(string username, string password)
    {
        Username = username;
        Password = password;
    }
    
    public async Task Start()
    {
        await StartTreesenseImporter();
        await StartParkingLotImporter();
    }
    
    
    private async Task StartParkingLotImporter()
    {
        // create a thread that calls the GetParkingLotSensorData method every minute
        var importerTimer = new Timer(async _ => await GetParkingLotSensorData(), null, 0, 60 * 1000);
        
        importerTimer?.Dispose();
    }
    
    private async Task StartTreesenseImporter()
    {
        
    }
    
    private async Task<TreesenseResponse> GetTreesenseSensorData()
    {
        return await ExecuteWithTryCatch<TreesenseResponse>(
            () => CreateRequest(Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.TreesenseEndpoint)),
            async response => await response.Content.ReadAsAsync<TreesenseResponse>()
        );
    }
    
    private async Task<ParkingLotResponse> GetParkingLotSensorData()
    {
        return await ExecuteWithTryCatch<ParkingLotResponse>(
            () => CreateRequest(Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.ParkingLotEndpoint)),
            async response => await response.Content.ReadAsAsync<ParkingLotResponse>()
        );
    }
    
    private HttpRequestMessage CreateRequest(string path)
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
        
        if (!_client.DefaultRequestHeaders.Contains("Authorization"))
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}");

        }
        
        return new HttpRequestMessage {
            RequestUri = new Uri(path),
            Version = new Version(2, 0) // has to be 1.1 for some older api's, here 2.0 worked 
        };
    }
    
    private async Task<T> ExecuteWithTryCatch<T>(Func<HttpRequestMessage> createRequest, Func<HttpResponseMessage, Task<T>> processResponse) where T : new()
    {
        T result = new();
        HttpRequestMessage request = createRequest();

        try
        {
            Console.WriteLine("sending request");
            HttpResponseMessage response = await _client!.SendAsync(request);
            Console.WriteLine("received response");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Processing response");
                var content = response.Content.ReadAsByteArrayAsync().Result;
                string stringencoding = System.Text.Encoding.UTF8.GetString(content, 0, content.Length);
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(stringencoding);
                Console.WriteLine(json);
                result = await processResponse(response);
                Console.WriteLine("Processed response");
            }
            else
            {
                Console.WriteLine(response.StatusCode);
            }
        }
        catch
        {
            Console.WriteLine("Error, rate limit perhaps");
        }

        return result;
    }
}