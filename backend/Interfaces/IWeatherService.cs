using PruebaTecnicaConfuturo.Domain.Aggregates;
using PruebaTecnicaConfuturo.Domain.Entities;

namespace PruebaTecnicaConfuturo.Interfaces;

public interface IWeatherService
{
    Task<ForecastReport> GetForecastAsync(Location location, CancellationToken cancellationToken = default);
}
