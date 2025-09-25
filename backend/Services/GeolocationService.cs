using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Domain.ValueObjects;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.External;
using PruebaTecnicaConfuturo.Models.Options;

namespace PruebaTecnicaConfuturo.Services;

public sealed class GeolocationService : IGeolocationService
{
    private static readonly string[] ForwardedHeaders = ["X-Forwarded-For", "X-Real-IP"];

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GeolocationApiOptions _options;
    private readonly ILogger<GeolocationService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public const string HttpClientName = "GeolocationApi";

    public GeolocationService(
        IHttpClientFactory httpClientFactory,
        IOptions<GeolocationApiOptions> options,
        ILogger<GeolocationService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Location> ResolveCurrentLocationAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.HasValidConfiguration)
        {
            _logger.LogWarning("Configuración de Geolocation API incompleta. Se devolverá ubicación simulada.");
            return Location.CreateFallback();
        }

        var client = _httpClientFactory.CreateClient(HttpClientName);
        var requestUri = BuildRequestUri(client, ResolveClientIpAddress());

        try
        {
            var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<GeolocationLookupResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                _logger.LogWarning("Respuesta vacía desde Geolocation API. Devolviendo ubicación simulada.");
                return Location.CreateFallback();
            }

            if (string.Equals(payload.Status, "fail", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Geolocation API indicó error: {Message}. Se devolverá ubicación simulada.", payload.Message);
                return Location.CreateFallback();
            }

            var latitude = payload.ResolveLatitude();
            var longitude = payload.ResolveLongitude();
            if (latitude is null || longitude is null)
            {
                _logger.LogWarning("La respuesta de Geolocation API no contiene coordenadas. Se devolverá ubicación simulada.");
                return Location.CreateFallback();
            }

            var coordinates = new Coordinates(latitude.Value, longitude.Value);
            if (!coordinates.IsValid)
            {
                _logger.LogWarning("Coordenadas inválidas recibidas desde Geolocation API. Devolviendo ubicación simulada.");
                return Location.CreateFallback();
            }

            return new Location
            {
                City = payload.City ?? "Ciudad desconocida",
                Region = payload.ResolveRegion(),
                Country = payload.ResolveCountry() ?? "País desconocido",
                Coordinates = coordinates
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar la Geolocation API. Se devolverá ubicación simulada.");
            return Location.CreateFallback();
        }
    }

    private Uri BuildRequestUri(HttpClient client, string? clientIp)
    {
        var baseUri = client.BaseAddress ?? new Uri(_options.BaseUrl!);
        var relativePath = string.IsNullOrWhiteSpace(clientIp) ? "json/" : $"{clientIp}/json/";
        var requestUri = new Uri(baseUri, relativePath);

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            var builder = new UriBuilder(requestUri);
            var existingQuery = builder.Query.TrimStart('?');
            var apiKeyParameter = string.IsNullOrWhiteSpace(_options.ApiKeyQueryParameter)
                ? "apiKey"
                : _options.ApiKeyQueryParameter;
            builder.Query = string.IsNullOrWhiteSpace(existingQuery)
                ? $"{apiKeyParameter}={_options.ApiKey}"
                : $"{existingQuery}&{apiKeyParameter}={_options.ApiKey}";
            requestUri = builder.Uri;
        }

        return requestUri;
    }

    private string? ResolveClientIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        foreach (var header in ForwardedHeaders)
        {
            if (!context.Request.Headers.TryGetValue(header, out var values))
            {
                continue;
            }

            var rawValues = values.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var candidate in rawValues)
            {
                if (IPAddress.TryParse(candidate, out var parsed) && IsPublicAddress(parsed))
                {
                    return parsed.ToString();
                }
            }
        }

        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is null)
        {
            return null;
        }

        return IsPublicAddress(remoteIp) ? remoteIp.ToString() : null;
    }

    private static bool IsPublicAddress(IPAddress address)
    {
        if (IPAddress.IsLoopback(address))
        {
            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = address.GetAddressBytes();
            return bytes[0] switch
            {
                10 => false,
                127 => false,
                172 when bytes[1] >= 16 && bytes[1] <= 31 => false,
                192 when bytes[1] == 168 => false,
                _ => true
            };
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (address.IsIPv6LinkLocal || address.IsIPv6Multicast)
            {
                return false;
            }

            var bytes = address.GetAddressBytes();
            if (bytes.Length >= 2 && bytes[0] == 0xFE && (bytes[1] & 0xC0) == 0x80)
            {
                return false;
            }

            if (bytes.Length >= 1 && (bytes[0] == 0xFC || bytes[0] == 0xFD))
            {
                return false;
            }
        }

        return true;
    }
}
