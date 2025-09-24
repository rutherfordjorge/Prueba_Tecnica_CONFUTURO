using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using PruebaTecnicaConfuturo.Domain.Aggregates;
using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Domain.ValueObjects;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.External;
using PruebaTecnicaConfuturo.Models.Options;

namespace PruebaTecnicaConfuturo.Services;

public sealed class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WeatherApiOptions _options;
    private readonly ILogger<WeatherService> _logger;

    public const string HttpClientName = "WeatherApi";

    public WeatherService(
        IHttpClientFactory httpClientFactory,
        IOptions<WeatherApiOptions> options,
        ILogger<WeatherService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ForecastReport> GetForecastAsync(Location location, CancellationToken cancellationToken = default)
    {
        if (!_options.HasValidConfiguration)
        {
            _logger.LogWarning("Configuración de Weather API incompleta. Se utilizará un pronóstico simulado.");
            return ForecastReport.CreateFallback(location);
        }

        var client = _httpClientFactory.CreateClient(HttpClientName);

        var query = new Dictionary<string, string?>
        {
            ["lat"] = location.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture),
            ["lon"] = location.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture),
            ["exclude"] = "minutely,hourly,alerts,current",
            ["appid"] = _options.ApiKey,
            ["units"] = _options.Units,
            ["lang"] = _options.Language
        };

        var uriBuilder = new UriBuilder(client.BaseAddress ?? new Uri(_options.BaseUrl!))
        {
            Path = "data/3.0/onecall",
            Query = string.Join("&", query.Select(kvp => $"{kvp.Key}={kvp.Value}"))
        };

        try
        {
            var response = await client.GetAsync(uriBuilder.Uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<OpenWeatherOneCallResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                _logger.LogWarning("Respuesta vacía desde Weather API. Retornando pronóstico simulado.");
                return ForecastReport.CreateFallback(location);
            }

            var daily = payload.Daily
                .Take(7)
                .Select(day => new DailyWeather
                {
                    Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(day.Dt).Date),
                    Temperature = new Temperature(Math.Round(day.Temperature.Day, 1)),
                    Summary = day.Weather.FirstOrDefault()?.Description ?? "Sin descripción",
                    Icon = day.Weather.FirstOrDefault()?.Icon
                })
                .ToList();

            if (!daily.Any())
            {
                _logger.LogWarning("Weather API no retornó datos diarios. Retornando pronóstico simulado.");
                return ForecastReport.CreateFallback(location);
            }

            return new ForecastReport
            {
                Location = location,
                Daily = daily
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar la Weather API. Se utilizará un pronóstico simulado.");
            return ForecastReport.CreateFallback(location);
        }
    }
}
