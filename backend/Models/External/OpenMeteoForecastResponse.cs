using System.Text.Json.Serialization;

namespace PruebaTecnicaConfuturo.Models.External;

public sealed class OpenMeteoForecastResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("daily")]
    public OpenMeteoDailyForecast? Daily { get; set; }
}

public sealed class OpenMeteoDailyForecast
{
    [JsonPropertyName("time")]
    public List<string>? Time { get; set; }

    [JsonPropertyName("temperature_2m_max")]
    public List<double?>? TemperatureMax { get; set; }

    [JsonPropertyName("temperature_2m_min")]
    public List<double?>? TemperatureMin { get; set; }

    [JsonPropertyName("weathercode")]
    public List<int?>? WeatherCode { get; set; }
}
