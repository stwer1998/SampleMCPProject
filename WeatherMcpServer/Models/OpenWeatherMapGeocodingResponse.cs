using System.Text.Json.Serialization;

namespace WeatherMcpServer.Models;

public record OpenWeatherMapGeocodingResponse(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("country")] string? Country
);
