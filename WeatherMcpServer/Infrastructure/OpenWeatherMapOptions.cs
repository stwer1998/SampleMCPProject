// OpenWeatherMapOptions.cs
namespace WeatherMcpServer.Infrastructure;

public class OpenWeatherMapOptions
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; }
    public string Units { get; set; }
    public string Lang { get; set; }
}
