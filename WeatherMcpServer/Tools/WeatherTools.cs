using System.ComponentModel;
using ModelContextProtocol.Server;
using WeatherMcpServer.Services;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Infrastructure;

namespace WeatherMcpServer.Tools;

/// <summary>
/// MCP tools for working with weather. Delegates business logic to the service.
/// </summary>
public class WeatherTools
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherTools> _logger;

    public WeatherTools(IWeatherService weatherService, ILogger<WeatherTools> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// Get weather forecast for a single city.
    /// </summary>
    [McpServerTool]
    [Description("Describes current weather in the provided city using OpenWeatherMap.")]
    public async Task<string> GetCityWeather(
        [Description("Name of the city to return weather for")] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return "City name must not be empty.";
        try
        {
            var forecast = await _weatherService.GetCityWeatherAsync(city);
            return $"The weather in {forecast.City}: {forecast.Description}, {forecast.TemperatureCelsius}°C ({forecast.TemperatureFahrenheit}°F), Humidity: {forecast.Humidity}%, Wind: {forecast.WindSpeed} m/s.";
        }
        catch (CityNotFoundException)
        {
            return $"City '{city}' not found.";
        }
        catch (OpenWeatherMapApiException ex)
        {
            _logger.LogError(ex, "API error for city {City}", city);
            return $"Failed to get weather for {city}: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error for city {City}", city);
            return $"Unexpected error: {ex.Message}";
        }
    }

    /// <summary>
    /// Get weather forecast for multiple cities.
    /// </summary>
    [McpServerTool]
    [Description("Describes current weather for multiple cities using OpenWeatherMap.")]
    public async Task<string[]> GetCitiesWeather(
        [Description("Names of the cities to return weather for")] string[] cities)
    {
        if (cities == null || cities.Length == 0)
            return new[] { "City list must not be empty." };
        try
        {
            var forecasts = await _weatherService.GetCitiesWeatherAsync(cities);
            return forecasts.Select(forecast =>
                $"The weather in {forecast.City}: {forecast.Description}, {forecast.TemperatureCelsius}°C ({forecast.TemperatureFahrenheit}°F), Humidity: {forecast.Humidity}%, Wind: {forecast.WindSpeed} m/s.").ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error for cities: {Cities}", string.Join(", ", cities));
            return new[] { $"Unexpected error: {ex.Message}" };
        }
    }
}