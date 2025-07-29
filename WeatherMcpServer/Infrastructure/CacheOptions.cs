namespace WeatherMcpServer.Infrastructure;

public class CacheOptions
{
    public int WeatherCacheSeconds { get; set; } = 3600;
    public int GeoCacheSeconds { get; set; } = 86400; // 24 hours by default
}
