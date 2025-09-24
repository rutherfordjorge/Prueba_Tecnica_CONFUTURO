using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Domain.ValueObjects;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.External;
using PruebaTecnicaConfuturo.Models.Options;

namespace PruebaTecnicaConfuturo.Services;

public sealed class GeolocationService : IGeolocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GeolocationApiOptions _options;
    private readonly ILogger<GeolocationService> _logger;

    public const string HttpClientName = "GeolocationApi";

    public GeolocationService(
        IHttpClientFactory httpClientFactory,
        IOptions<GeolocationApiOptions> options,
        ILogger<GeolocationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Location> ResolveCurrentLocationAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.HasValidConfiguration)
        {
            _logger.LogWarning("Configuración de Geolocation API incompleta. Se devolverá ubicación simulada.");
            return Location.CreateFallback();
        }

        var client = _httpClientFactory.CreateClient(HttpClientName);
        var uriBuilder = new UriBuilder(client.BaseAddress ?? new Uri(_options.BaseUrl!))
        {
            Path = "ipgeo",
            Query = $"apiKey={_options.ApiKey}"
        };

        try
        {
            var response = await client.GetAsync(uriBuilder.Uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<IpGeolocationResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                _logger.LogWarning("Respuesta vacía desde Geolocation API. Devolviendo ubicación simulada.");
                return Location.CreateFallback();
            }

            var coordinates = new Coordinates(payload.Latitude, payload.Longitude);
            if (!coordinates.IsValid)
            {
                _logger.LogWarning("Coordenadas inválidas recibidas desde Geolocation API. Devolviendo ubicación simulada.");
                return Location.CreateFallback();
            }

            return new Location
            {
                City = payload.City ?? "Ciudad desconocida",
                Region = payload.Region,
                Country = payload.Country ?? "País desconocido",
                Coordinates = coordinates
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar la Geolocation API. Se devolverá ubicación simulada.");
            return Location.CreateFallback();
        }
    }
}
