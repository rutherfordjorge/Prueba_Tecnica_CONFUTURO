using PruebaTecnicaConfuturo.Domain.Entities;

namespace PruebaTecnicaConfuturo.Models.Dtos;

public sealed record LocationResponseDto
{
    public required string City { get; init; }
    public string? Region { get; init; }
    public required string Country { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }

    public static LocationResponseDto FromDomain(Location location) => new()
    {
        City = location.City,
        Region = location.Region,
        Country = location.Country,
        Latitude = location.Coordinates.Latitude,
        Longitude = location.Coordinates.Longitude
    };
}
