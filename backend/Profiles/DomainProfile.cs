using AutoMapper;
using PruebaTecnicaConfuturo.Domain.Aggregates;
using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Models.Dtos;

namespace PruebaTecnicaConfuturo.Profiles;

public sealed class DomainProfile : Profile
{
    public DomainProfile()
    {
        CreateMap<Location, LocationResponseDto>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coordinates.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coordinates.Longitude));

        CreateMap<Location, ForecastResponseDto.LocationDto>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coordinates.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coordinates.Longitude));

        CreateMap<DailyWeather, ForecastResponseDto.DailyWeatherDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.TemperatureC, opt => opt.MapFrom(src => src.Temperature.Celsius))
            .ForMember(dest => dest.TemperatureF, opt => opt.MapFrom(src => src.Temperature.Fahrenheit));

        CreateMap<ForecastReport, ForecastResponseDto>();
    }
}
