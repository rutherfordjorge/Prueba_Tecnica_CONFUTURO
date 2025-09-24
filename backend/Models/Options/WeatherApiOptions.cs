namespace PruebaTecnicaConfuturo.Models.Options;

public sealed class WeatherApiOptions
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string Units { get; set; } = "metric";
    public string Language { get; set; } = "es";

    public bool HasValidConfiguration => !string.IsNullOrWhiteSpace(BaseUrl) && !string.IsNullOrWhiteSpace(ApiKey);
}
