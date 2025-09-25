using System.Text.Json.Serialization;

namespace PruebaTecnicaConfuturo.Models.External;

public sealed class OpenMeteoArchiveResponse
{
    [JsonPropertyName("daily")]
    public OpenMeteoArchiveDaily? Daily { get; set; }
}

public sealed class OpenMeteoArchiveDaily
{
    [JsonPropertyName("time")]
    public List<string>? Time { get; set; }

    [JsonPropertyName("temperature_2m_mean")]
    public List<double?>? TemperatureMean { get; set; }

    [JsonPropertyName("weathercode")]
    public List<int?>? WeatherCode { get; set; }
}
