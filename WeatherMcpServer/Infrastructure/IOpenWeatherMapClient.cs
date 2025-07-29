using WeatherMcpServer.Models;

namespace WeatherMcpServer.Infrastructure;

public interface IOpenWeatherMapClient
{
    Task<(double lat, double lon)> GetCityCoordinatesAsync(string city, CancellationToken cancellationToken = default);
    Task<OpenWeatherMapWeatherResponse> GetWeatherAsync(double lat, double lon, CancellationToken cancellationToken = default);
}
