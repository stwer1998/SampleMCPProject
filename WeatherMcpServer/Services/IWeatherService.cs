using WeatherMcpServer.Models;

namespace WeatherMcpServer.Services;

public interface IWeatherService
{
    Task<WeatherForecast> GetCityWeatherAsync(string city, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeatherForecast>> GetCitiesWeatherAsync(IEnumerable<string> cities, CancellationToken cancellationToken = default);
}
