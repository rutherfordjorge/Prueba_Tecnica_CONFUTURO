using PruebaTecnicaConfuturo.Domain.ValueObjects;

namespace PruebaTecnicaConfuturo.Domain.Entities;

public sealed record Location
{
    public required string City { get; init; }
    public string? Region { get; init; }
    public required string Country { get; init; }
    public required Coordinates Coordinates { get; init; }

    public static Location CreateFallback() => new()
    {
        City = "Santiago",
        Region = "Metropolitana",
        Country = "Chile",
        Coordinates = new Coordinates(-33.4489, -70.6693)
    };
}
