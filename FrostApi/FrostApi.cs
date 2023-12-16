using System.Net;
using System.Text;
using FrostApi.Endpoints;
using FrostApi.Endpoints.Things;
using Microsoft.Extensions.Logging;

namespace FrostApi;

public class FrostApi
{
    private FrostHttpClient? _client;
    private Constants.Endpoints _endpoints;
    public Things Things { get; set;  }
    
    public FrostApi(string baseUrl)
    {
        _client = new FrostHttpClient();
        _endpoints = new Constants.Endpoints(baseUrl);
        
        Things = new Things(_client, _endpoints);
    }
    
}