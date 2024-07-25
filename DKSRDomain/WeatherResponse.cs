using Newtonsoft.Json;

namespace DKSRDomain;

public class WeatherResponse
{
    public List<WeatherSensorData> SensorData { get; set; } = null!;
}

// {
//     "rain": 0,
//     "weather_id": 800,
//     "temp": 299.37,
//     "sunrise": "2024-07-19T03:24:11.000Z",
//     "visibility": 10000,
//     "city": "Weißdorf",
//     "countrycode": "DE",
//     "lon": 11.85,
//     "clouds": 0,
//     "pressure": 1018,
//     "weather_icon": "01d",
//     "feels_like": 299.37,
//     "temp_max": 299.37,
//     "SID": "fbc81ae8-87a2-49e7-948e-aeef30e66f71",
//     "wind_deg": 190,
//     "temp_min": 299.26,
//     "grnd_level": 949,
//     "sunset": "2024-07-19T19:13:15.000Z",
//     "humidity": 53,
//     "wind_speed": 2.06,
//     "sea_level": 1018,
//     "lat": 50.1833,
//     "timestamp": "2024-07-19T15:43:50.730Z",
//     "_headers": {
//         "eventType": "OpenWeatherMapHofV1EventType"
//     }
// }

public class WeatherSensorData
{
    [JsonProperty("rain")] public int Rain { get; set; }
    
    [JsonProperty("weather_id")] public int WeatherId { get; set; }
    
    [JsonProperty("temp")] public double Temp { get; set; }
    
    [JsonProperty("sunrise")] public DateTime Sunrise { get; set; }
    
    [JsonProperty("visibility")] public int Visibility { get; set; }
    
    [JsonProperty("city")] public string City { get; set; } = null!;

    [JsonProperty("countrycode")] public string CountryCode { get; set; } = null!;
    
    [JsonProperty("lon")] public double Lon { get; set; }
    
    [JsonProperty("clouds")] public int Clouds { get; set; }
    
    [JsonProperty("pressure")] public int Pressure { get; set; }
    
    [JsonProperty("weather_icon")] public string WeatherIcon { get; set; } = null!;
    
    [JsonProperty("feels_like")] public double FeelsLike { get; set; }
    
    [JsonProperty("temp_max")] public double TempMax { get; set; }
    
    [JsonProperty("SID")] public string SID { get; set; } = null!;
    
    [JsonProperty("wind_deg")] public int WindDeg { get; set; }
    
    [JsonProperty("temp_min")] public double TempMin { get; set; }
    
    [JsonProperty("grnd_level")] public int GrndLevel { get; set; }
    
    [JsonProperty("sunset")] public DateTime Sunset { get; set; }
    
    [JsonProperty("humidity")] public int Humidity { get; set; }
    
    [JsonProperty("wind_speed")] public double WindSpeed { get; set; }
    
    [JsonProperty("sea_level")] public int SeaLevel { get; set; }
    
    [JsonProperty("lat")] public double Lat { get; set; }
    
    [JsonProperty("timestamp")] public DateTime Timestamp { get; set; }
    
    [JsonProperty("_headers")] public Dictionary<string, string> Headers { get; set; } = null!;
}