using System.Text.Json.Serialization;

namespace PruebaTecnicaConfuturo.Models.External;

public sealed class IpGeolocationResponse
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state_prov")]
    public string? Region { get; set; }

    [JsonPropertyName("country_name")]
    public string? Country { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}
