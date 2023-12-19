using System.Net;
using System.Text;
using DKSRDomain;
using Importer.Constants;
using Microsoft.Extensions.Logging;

namespace Importer.Importers;

public class EndpointImporter
{
    public readonly ILogger _logger;
    private HttpClient? _client;

    public EndpointImporter(ILogger logger, string username, string password)
    {
        Username = username;
        Password = password;
        _logger = logger;
    }

    public static string Username { get; set; }
    public static string Password { get; set; }

    public async Task Start()
    {
        await SetupHttpClient();
        await StartTreesenseImporter();
        await StartParkingLotImporter();
    }

    private async Task SetupHttpClient()
    {
        if (_client == null)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            };
            _client = new HttpClient(handler);
        }

        if (!_client.DefaultRequestHeaders.Contains("Accept"))
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

        if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

        if (!_client.DefaultRequestHeaders.Contains("Accept-Encoding"))
            _client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

        if (!_client.DefaultRequestHeaders.Contains("Accept-Language"))
            _client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,nl;q=0.8");

        if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            _client.DefaultRequestHeaders.Add("Authorization",
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}");
    }

    private async Task StartTreesenseImporter()
    {
        _logger.LogInformation("Starting TreeSense Sensor Data Collection");
        var importerTimer = new Timer(async _ => await GetTreesenseSensorData(), null, 0, 60 * 1000 * 60);
    }

    private async Task StartParkingLotImporter()
    {
        _logger.LogInformation("Starting Parking Lot Sensor Data Collection");
        var importerTimer = new Timer(async _ => await GetParkingLotSensorData(), null, 0, 60 * 1000);
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
            () => CreateRequest(
                Endpoints.GetAuthenticatedEndpointUrl(Username, Password, Endpoints.ParkingLotEndpoint)),
            async response => await response.Content.ReadAsAsync<ParkingLotResponse>()
        );
    }

    private HttpRequestMessage CreateRequest(string path)
    {
        return new HttpRequestMessage
        {
            RequestUri = new Uri(path),
            Version = new Version(2, 0) // has to be 1.1 for some older api's, here 2.0 worked 
        };
    }

    private async Task<T> ExecuteWithTryCatch<T>(Func<HttpRequestMessage> createRequest,
        Func<HttpResponseMessage, Task<T>> processResponse) where T : new()
    {
        T result = new();
        var request = createRequest();

        try
        {
            _logger.LogDebug("sending request");
            var response = await _client!.SendAsync(request);
            _logger.LogDebug("received response");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Processing response");
                result = await processResponse(response);
                _logger.LogDebug("Processed response");
            }
            else
            {
                _logger.LogWarning(response.StatusCode.ToString());
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }

        return result;
    }
}