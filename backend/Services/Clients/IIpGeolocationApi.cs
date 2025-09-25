using Refit;
using PruebaTecnicaConfuturo.Models.External;

namespace PruebaTecnicaConfuturo.Services.Clients;

public interface IIpGeolocationApi
{
    [Get("/ipgeo")]
    Task<IpGeolocationResponse> ResolveAsync(
        [AliasAs("apiKey")] string apiKey,
        [AliasAs("ip")] string? ipAddress,
        CancellationToken cancellationToken = default);
}
