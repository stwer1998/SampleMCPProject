using AutoMapper;
using WeatherMcpServer.Models;

namespace WeatherMcpServer.Mappers;

public class WeatherProfile : Profile
{
    public WeatherProfile()
    {
        CreateMap<(string city, OpenWeatherMapWeatherResponse response), WeatherForecast>()
            .ForCtorParam("City", opt => opt.MapFrom(src => src.city))
            .ForCtorParam("Description", opt => opt.MapFrom(src => (src.response.Weather != null && src.response.Weather.Length > 0) ? src.response.Weather[0].Description : string.Empty))
            .ForCtorParam("TemperatureCelsius", opt => opt.MapFrom(src => src.response.Main.Temp))
            .ForCtorParam("TemperatureFahrenheit", opt => opt.MapFrom(src => src.response.Main.Temp * 9 / 5 + 32))
            .ForCtorParam("Humidity", opt => opt.MapFrom(src => src.response.Main.Humidity))
            .ForCtorParam("WindSpeed", opt => opt.MapFrom(src => src.response.Wind.Speed));
    }
}
