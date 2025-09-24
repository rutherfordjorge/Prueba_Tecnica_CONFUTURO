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

        try
        {
            var upcoming = await FetchUpcomingAsync(client, location, cancellationToken);
            var historical = await FetchHistoricalAsync(client, location, cancellationToken);

            if (!upcoming.Any())
            {
                _logger.LogWarning("Weather API no retornó datos diarios futuros. Retornando pronóstico simulado.");
                return ForecastReport.CreateFallback(location);
            }

            if (!historical.Any())
            {
                _logger.LogWarning("Weather API no retornó datos históricos. Retornando pronóstico simulado.");
                return ForecastReport.CreateFallback(location);
            }

            return new ForecastReport
            {
                Location = location,
                Daily = upcoming,
                Historical = historical
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar la Weather API. Se utilizará un pronóstico simulado.");
            return ForecastReport.CreateFallback(location);
        }
    }

    private async Task<IReadOnlyCollection<DailyWeather>> FetchUpcomingAsync(HttpClient client, Location location, CancellationToken cancellationToken)
    {
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

        var response = await client.GetAsync(uriBuilder.Uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OpenWeatherOneCallResponse>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return Array.Empty<DailyWeather>();
        }

        return payload.Daily
            .Take(7)
            .Select(day => new DailyWeather
            {
                Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(day.Dt).Date),
                Temperature = new Temperature(Math.Round(day.Temperature.Day, 1)),
                Summary = day.Weather.FirstOrDefault()?.Description ?? "Sin descripción",
                Icon = day.Weather.FirstOrDefault()?.Icon
            })
            .ToList();
    }

    private async Task<IReadOnlyCollection<DailyWeather>> FetchHistoricalAsync(HttpClient client, Location location, CancellationToken cancellationToken)
    {
        var baseUri = client.BaseAddress ?? new Uri(_options.BaseUrl!);
        var dates = Enumerable.Range(1, 7)
            .Select(offset => DateTimeOffset.UtcNow.Date.AddDays(-offset).AddHours(12))
            .ToArray();

        var tasks = dates.Select(async date =>
        {
            var query = new Dictionary<string, string?>
            {
                ["lat"] = location.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture),
                ["lon"] = location.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture),
                ["dt"] = date.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                ["appid"] = _options.ApiKey,
                ["units"] = _options.Units,
                ["lang"] = _options.Language
            };

            var uriBuilder = new UriBuilder(baseUri)
            {
                Path = "data/3.0/onecall/timemachine",
                Query = string.Join("&", query.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            };

            try
            {
                var response = await client.GetAsync(uriBuilder.Uri, cancellationToken);
                response.EnsureSuccessStatusCode();

                var payload = await response.Content.ReadFromJsonAsync<OpenWeatherTimeMachineResponse>(cancellationToken: cancellationToken);
                if (payload?.Data is null || payload.Data.Count == 0)
                {
                    return null;
                }

                var averageTemperature = Math.Round(payload.Data.Average(item => item.Temperature), 1);
                var weatherDescription = payload.Data
                    .SelectMany(item => item.Weather)
                    .FirstOrDefault();

                return new DailyWeather
                {
                    Date = DateOnly.FromDateTime(date.Date),
                    Temperature = new Temperature(averageTemperature),
                    Summary = weatherDescription?.Description ?? "Sin descripción",
                    Icon = weatherDescription?.Icon
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener el clima histórico para {Date}", date.Date.ToShortDateString());
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        return results
            .Where(item => item is not null)
            .Select(item => item!)
            .OrderBy(item => item.Date)
            .ToList();
    }
}
