// WeatherForecast.cs
namespace WeatherMcpServer.Models;

public record WeatherForecast(string City, string Description, double TemperatureCelsius, double TemperatureFahrenheit, int Humidity, double WindSpeed);
