using PruebaTecnicaConfuturo.Domain.Entities;

namespace PruebaTecnicaConfuturo.Interfaces;

public interface IGeolocationService
{
    Task<Location> ResolveCurrentLocationAsync(CancellationToken cancellationToken = default);
}
