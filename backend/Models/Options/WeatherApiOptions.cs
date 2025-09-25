namespace PruebaTecnicaConfuturo.Models.Options;

public sealed class WeatherApiOptions
{
    public string? ForecastBaseUrl { get; set; } = "https://api.open-meteo.com/";
    public string? HistoricalBaseUrl { get; set; } = "https://archive-api.open-meteo.com/";
    public string Timezone { get; set; } = "auto";

    public bool HasValidConfiguration =>
        !string.IsNullOrWhiteSpace(ForecastBaseUrl) &&
        !string.IsNullOrWhiteSpace(HistoricalBaseUrl);
}
