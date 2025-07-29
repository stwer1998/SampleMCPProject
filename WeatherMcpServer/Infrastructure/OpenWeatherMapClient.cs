using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Infrastructure;

/// <summary>
/// Client for working with the OpenWeatherMap API.
/// </summary>
public class OpenWeatherMapClient : IOpenWeatherMapClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherMapOptions _options;
    private readonly ILogger<OpenWeatherMapClient> _logger;
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _cacheOptions;

    public OpenWeatherMapClient(HttpClient httpClient, IOptions<OpenWeatherMapOptions> options, ILogger<OpenWeatherMapClient> logger, IMemoryCache cache, IOptions<CacheOptions> cacheOptions)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _cache = cache;
        _cacheOptions = cacheOptions.Value;
    }

    /// <inheritdoc/>
    public async Task<(double lat, double lon)> GetCityCoordinatesAsync(string city, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"geo:{city.ToLowerInvariant()}";
        if (_cache.TryGetValue<(double lat, double lon)>(cacheKey, out var cached))
        {
            _logger.LogInformation("Geo cache hit for city {City}", city);
            return cached;
        }
        _logger.LogInformation("Geo cache miss for city {City}", city);
        var url = $"{_options.BaseUrl}/geo/1.0/direct?q={Uri.EscapeDataString(city)}&limit=1&appid={_options.ApiKey}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Geocoding request failed for city {City} with status {Status}. Response: {Body}", city, response.StatusCode, json);
            throw new OpenWeatherMapApiException($"Failed to get coordinates for city '{city}'. Status: {response.StatusCode}");
        }
        var geo = JsonSerializer.Deserialize<List<OpenWeatherMapGeocodingResponse>>(json);
        if (geo == null || geo.Count == 0)
        {
            throw new CityNotFoundException(city);
        }
        var result = (geo[0].Lat, geo[0].Lon);
        _cache.Set(cacheKey, result, TimeSpan.FromSeconds(_cacheOptions.GeoCacheSeconds));
        return result;
    }

    /// <inheritdoc/>
    public async Task<OpenWeatherMapWeatherResponse> GetWeatherAsync(double lat, double lon, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/data/2.5/weather?lat={lat}&lon={lon}&appid={_options.ApiKey}&units={_options.Units}&lang={_options.Lang}";
        _logger.LogInformation("Requesting weather for lat={Lat}, lon={Lon}", lat, lon);
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Weather request failed for lat={Lat}, lon={Lon} with status {Status}. Response: {Body}", lat, lon, response.StatusCode, json);
            throw new OpenWeatherMapApiException($"Failed to get weather for coordinates {lat},{lon}. Status: {response.StatusCode}");
        }
        var weather = JsonSerializer.Deserialize<OpenWeatherMapWeatherResponse>(json);
        if (weather == null)
        {
            _logger.LogError("Weather response is null. Raw body: {Body}", json);
            throw new OpenWeatherMapApiException("Weather response is null");
        }
        return weather;
    }
}
