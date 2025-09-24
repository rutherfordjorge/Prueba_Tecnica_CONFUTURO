using System.Text.Json.Serialization;

namespace PruebaTecnicaConfuturo.Models.External;

public sealed class OpenWeatherTimeMachineResponse
{
    [JsonPropertyName("data")]
    public List<HistoricalData> Data { get; set; } = [];

    public sealed class HistoricalData
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherDescription> Weather { get; set; } = [];
    }

    public sealed class WeatherDescription
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }
}
