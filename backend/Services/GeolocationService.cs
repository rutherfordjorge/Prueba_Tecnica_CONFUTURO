using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Domain.ValueObjects;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.Options;
using PruebaTecnicaConfuturo.Services.Clients;

namespace PruebaTecnicaConfuturo.Services;

public sealed class GeolocationService : IGeolocationService
{
    private readonly IIpGeolocationApi _apiClient;
    private readonly GeolocationApiOptions _options;
    private readonly ILogger<GeolocationService> _logger;

    public GeolocationService(
        IIpGeolocationApi apiClient,
        IOptions<GeolocationApiOptions> options,
        ILogger<GeolocationService> logger)
    {
        _apiClient = apiClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Location> ResolveAsync(string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (!_options.HasValidConfiguration)
        {
            _logger.LogWarning("Configuración de Geolocation API incompleta. Se utilizará la ubicación por defecto.");
            return Location.CreateFallback();
        }

        try
        {
            var response = await _apiClient.ResolveAsync(_options.ApiKey!, NormalizeIp(ipAddress), cancellationToken);

            if (response.Latitude is null || response.Longitude is null)
            {
                _logger.LogWarning("Geolocation API retornó coordenadas inválidas. Se retorna la ubicación por defecto.");
                return Location.CreateFallback();
            }

            return new Location
            {
                City = !string.IsNullOrWhiteSpace(response.City) ? response.City! : "Ubicación detectada",
                Region = response.State,
                Country = response.Country ?? string.Empty,
                Coordinates = new Coordinates(response.Latitude.Value, response.Longitude.Value)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consultando la Geolocation API. Se retorna la ubicación por defecto.");
            return Location.CreateFallback();
        }
    }

    private static string? NormalizeIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return null;
        }

        return ipAddress == "::1" ? "127.0.0.1" : ipAddress;
    }
}
