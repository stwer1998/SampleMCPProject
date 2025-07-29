using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Infrastructure;
using WeatherMcpServer.Models;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace WeatherMcpServer.Services;

/// <summary>
/// Business logic service for getting weather forecast with caching and logging.
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly IOpenWeatherMapClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeatherService> _logger;
    private readonly IMapper _mapper;
    private readonly CacheOptions _cacheOptions;

    public WeatherService(IOpenWeatherMapClient client, IMemoryCache cache, ILogger<WeatherService> logger, IMapper mapper, IOptions<CacheOptions> cacheOptions)
    {
        _client = client;
        _cache = cache;
        _logger = logger;
        _mapper = mapper;
        _cacheOptions = cacheOptions.Value;
    }

    /// <inheritdoc/>
    public async Task<WeatherForecast> GetCityWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"weather:{city.ToLowerInvariant()}";
        if (_cache.TryGetValue<WeatherForecast>(cacheKey, out var cached))
        {
            _logger.LogInformation("Cache hit for city {City}", city);
            return cached!;
        }
        _logger.LogInformation("Cache miss for city {City}", city);
        return await RefreshWeatherAsync(city, cacheKey, cancellationToken);
    }

    private async Task<WeatherForecast> RefreshWeatherAsync(string city, string cacheKey, CancellationToken cancellationToken)
    {
        try
        {
            var (lat, lon) = await _client.GetCityCoordinatesAsync(city, cancellationToken);
            var weatherResponse = await _client.GetWeatherAsync(lat, lon, cancellationToken);
            var forecast = _mapper.Map<WeatherForecast>((city, weatherResponse));
            _cache.Set(cacheKey, forecast, TimeSpan.FromSeconds(_cacheOptions.WeatherCacheSeconds));
            return forecast;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh weather for city {City}", city);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<WeatherForecast>> GetCitiesWeatherAsync(IEnumerable<string> cities, CancellationToken cancellationToken = default)
    {
        var tasks = cities.Select(city => GetCityWeatherAsync(city, cancellationToken));
        return await Task.WhenAll(tasks);
    }
}
