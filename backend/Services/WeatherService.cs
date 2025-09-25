using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
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

    private static readonly IReadOnlyDictionary<int, string> WeatherCodeDescriptions = new Dictionary<int, string>
    {
        [0] = "Despejado",
        [1] = "Principalmente despejado",
        [2] = "Parcialmente nublado",
        [3] = "Nublado",
        [45] = "Niebla",
        [48] = "Niebla con escarcha",
        [51] = "Llovizna ligera",
        [53] = "Llovizna moderada",
        [55] = "Llovizna intensa",
        [56] = "Llovizna helada ligera",
        [57] = "Llovizna helada intensa",
        [61] = "Lluvia ligera",
        [63] = "Lluvia moderada",
        [65] = "Lluvia intensa",
        [66] = "Lluvia helada ligera",
        [67] = "Lluvia helada intensa",
        [71] = "Nieve ligera",
        [73] = "Nieve moderada",
        [75] = "Nieve intensa",
        [77] = "Granizo",
        [80] = "Chubascos ligeros",
        [81] = "Chubascos moderados",
        [82] = "Chubascos intensos",
        [85] = "Chubascos de nieve ligeros",
        [86] = "Chubascos de nieve intensos",
        [95] = "Tormenta eléctrica",
        [96] = "Tormenta eléctrica con granizo ligero",
        [99] = "Tormenta eléctrica con granizo intenso"
    };

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
            ["latitude"] = location.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture),
            ["longitude"] = location.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture),
            ["timezone"] = string.IsNullOrWhiteSpace(_options.Timezone) ? "auto" : _options.Timezone,
            ["daily"] = "temperature_2m_max,temperature_2m_min,weathercode"
        };

        var baseUri = client.BaseAddress ?? new Uri(_options.ForecastBaseUrl!);
        var path = QueryHelpers.AddQueryString("v1/forecast", query);

        var response = await client.GetAsync(new Uri(baseUri, path), cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OpenMeteoForecastResponse>(cancellationToken: cancellationToken);
        if (payload?.Daily?.Time is null || payload.Daily.Time.Count == 0)
        {
            return Array.Empty<DailyWeather>();
        }

        var maxTemperatures = payload.Daily.TemperatureMax ?? new List<double?>();
        var minTemperatures = payload.Daily.TemperatureMin ?? new List<double?>();
        var weatherCodes = payload.Daily.WeatherCode ?? new List<int?>();

        var results = new List<DailyWeather>();
        var items = Math.Min(7, payload.Daily.Time.Count);

        for (var index = 0; index < items; index++)
        {
            if (!DateOnly.TryParse(payload.Daily.Time[index], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                continue;
            }

            var maxTemperature = index < maxTemperatures.Count ? maxTemperatures[index] : null;
            var minTemperature = index < minTemperatures.Count ? minTemperatures[index] : null;
            var temperatureValue = CalculateAverageTemperature(maxTemperature, minTemperature);

            if (temperatureValue is null)
            {
                continue;
            }

            var weatherCode = index < weatherCodes.Count ? weatherCodes[index] : null;

            results.Add(new DailyWeather
            {
                Date = date,
                Temperature = new Temperature(Math.Round(temperatureValue.Value, 1)),
                Summary = GetSummary(weatherCode),
                Icon = weatherCode?.ToString(CultureInfo.InvariantCulture)
            });
        }

        return results;
    }

    private async Task<IReadOnlyCollection<DailyWeather>> FetchHistoricalAsync(HttpClient client, Location location, CancellationToken cancellationToken)
    {
        var timezone = string.IsNullOrWhiteSpace(_options.Timezone) ? "auto" : _options.Timezone;
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1));
        var startDate = endDate.AddDays(-6);

        var query = new Dictionary<string, string?>
        {
            ["latitude"] = location.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture),
            ["longitude"] = location.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture),
            ["timezone"] = timezone,
            ["start_date"] = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["end_date"] = endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["daily"] = "temperature_2m_mean,weathercode"
        };

        var historicalBase = !string.IsNullOrWhiteSpace(_options.HistoricalBaseUrl)
            ? new Uri(_options.HistoricalBaseUrl)
            : client.BaseAddress ?? new Uri("https://archive-api.open-meteo.com/");

        var path = QueryHelpers.AddQueryString("v1/archive", query);

        var response = await client.GetAsync(new Uri(historicalBase, path), cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OpenMeteoArchiveResponse>(cancellationToken: cancellationToken);
        if (payload?.Daily?.Time is null || payload.Daily.Time.Count == 0)
        {
            return Array.Empty<DailyWeather>();
        }

        var times = payload.Daily.Time;
        var temperatures = payload.Daily.TemperatureMean ?? new List<double?>();
        var weatherCodes = payload.Daily.WeatherCode ?? new List<int?>();

        var results = new List<DailyWeather>();

        for (var index = 0; index < times.Count; index++)
        {
            if (!DateOnly.TryParse(times[index], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                continue;
            }

            var temperatureValue = index < temperatures.Count ? temperatures[index] : null;
            if (temperatureValue is null)
            {
                continue;
            }

            var weatherCode = index < weatherCodes.Count ? weatherCodes[index] : null;

            results.Add(new DailyWeather
            {
                Date = date,
                Temperature = new Temperature(Math.Round(temperatureValue.Value, 1)),
                Summary = GetSummary(weatherCode),
                Icon = weatherCode?.ToString(CultureInfo.InvariantCulture)
            });
        }

        return results
            .OrderBy(item => item.Date)
            .ToList();
    }

    private static double? CalculateAverageTemperature(double? maxTemperature, double? minTemperature)
    {
        if (maxTemperature.HasValue && minTemperature.HasValue)
        {
            return (maxTemperature.Value + minTemperature.Value) / 2;
        }

        if (maxTemperature.HasValue)
        {
            return maxTemperature.Value;
        }

        if (minTemperature.HasValue)
        {
            return minTemperature.Value;
        }

        return null;
    }

    private static string GetSummary(int? weatherCode)
    {
        if (weatherCode is null)
        {
            return "Sin descripción";
        }

        return WeatherCodeDescriptions.TryGetValue(weatherCode.Value, out var description)
            ? description
            : $"Código meteorológico {weatherCode.Value}";
    }
}
