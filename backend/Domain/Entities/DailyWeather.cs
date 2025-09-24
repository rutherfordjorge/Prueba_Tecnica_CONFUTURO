using PruebaTecnicaConfuturo.Domain.ValueObjects;

namespace PruebaTecnicaConfuturo.Domain.Entities;

public sealed record DailyWeather
{
    public required DateOnly Date { get; init; }
    public required Temperature Temperature { get; init; }
    public required string Summary { get; init; }
    public string? Icon { get; init; }
}
