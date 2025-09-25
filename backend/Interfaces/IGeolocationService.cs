using PruebaTecnicaConfuturo.Domain.Entities;

namespace PruebaTecnicaConfuturo.Interfaces;

public interface IGeolocationService
{
    Task<Location> ResolveAsync(string? ipAddress, CancellationToken cancellationToken = default);
}
