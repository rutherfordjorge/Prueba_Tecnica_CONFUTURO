using PruebaTecnicaConfuturo.Domain.Aggregates;
using PruebaTecnicaConfuturo.Domain.Entities;

namespace PruebaTecnicaConfuturo.Models.Dtos;

public sealed record ForecastResponseDto
{
    public required LocationDto Location { get; init; }
    public required IReadOnlyCollection<DailyWeatherDto> Daily { get; init; }

    public static ForecastResponseDto FromDomain(ForecastReport report) => new()
    {
        Location = LocationDto.FromDomain(report.Location),
        Daily = report.Daily.Select(DailyWeatherDto.FromDomain).ToList(),
    };

    public sealed record LocationDto
    {
        public required string City { get; init; }
        public string? Region { get; init; }
        public required string Country { get; init; }
        public required double Latitude { get; init; }
        public required double Longitude { get; init; }

        public static LocationDto FromDomain(Location location) => new()
        {
            City = location.City,
            Region = location.Region,
            Country = location.Country,
            Latitude = location.Coordinates.Latitude,
            Longitude = location.Coordinates.Longitude
        };
    }

    public sealed record DailyWeatherDto
    {
        public required string Date { get; init; }
        public required double TemperatureC { get; init; }
        public required double TemperatureF { get; init; }
        public required string Summary { get; init; }
        public string? Icon { get; init; }

        public static DailyWeatherDto FromDomain(DailyWeather weather) => new()
        {
            Date = weather.Date.ToString("yyyy-MM-dd"),
            TemperatureC = weather.Temperature.Celsius,
            TemperatureF = weather.Temperature.Fahrenheit,
            Summary = weather.Summary,
            Icon = weather.Icon
        };
    }
}
