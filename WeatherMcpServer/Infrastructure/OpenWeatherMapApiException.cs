namespace WeatherMcpServer.Infrastructure;

public class OpenWeatherMapApiException : Exception
{
    public OpenWeatherMapApiException(string message) : base(message) { }
    public OpenWeatherMapApiException(string message, Exception inner) : base(message, inner) { }
}

public class CityNotFoundException : OpenWeatherMapApiException
{
    public CityNotFoundException(string city) : base($"City '{city}' not found.") { }
}
