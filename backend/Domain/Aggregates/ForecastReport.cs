using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Domain.ValueObjects;

namespace PruebaTecnicaConfuturo.Domain.Aggregates;

public sealed record ForecastReport
{
    public required Location Location { get; init; }
    public required IReadOnlyCollection<DailyWeather> Daily { get; init; }
    public required IReadOnlyCollection<DailyWeather> Historical { get; init; }

    public static ForecastReport CreateFallback(Location location)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var random = new Random(42);
        var summaries = new[]
        {
            "Despejado", "Parcialmente nublado", "Lluvia ligera", "Tormenta", "Nublado"
        };

        var upcoming = Enumerable.Range(0, 7)
            .Select(offset => new DailyWeather
            {
                Date = today.AddDays(offset),
                Temperature = new Temperature(random.Next(8, 28)),
                Summary = summaries[offset % summaries.Length],
                Icon = "01d"
            })
            .ToList();

        var historical = Enumerable.Range(1, 8)
            .Select(offset => new DailyWeather
            {
                Date = today.AddDays(-offset),
                Temperature = new Temperature(random.Next(8, 28)),
                Summary = summaries[(offset + 2) % summaries.Length],
                Icon = "02n"
            })
            .OrderBy(item => item.Date)
            .ToList();

        return new ForecastReport
        {
            Location = location,
            Daily = upcoming,
            Historical = historical
        };
    }
}
