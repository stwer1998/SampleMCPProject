// OpenWeatherMapWeatherResponse.cs
using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

public record OpenWeatherMapWeatherResponse(
    [property: JsonPropertyName("weather")] WeatherDescription[] Weather,
    [property: JsonPropertyName("main")] MainInfo Main,
    [property: JsonPropertyName("wind")] WindInfo Wind
);

public record WeatherDescription(
    [property: JsonPropertyName("description")] string Description
);

public record MainInfo(
    [property: JsonPropertyName("temp")] double Temp,
    [property: JsonPropertyName("humidity")] int Humidity
);

public record WindInfo(
    [property: JsonPropertyName("speed")] double Speed
);
