using System.Text.Json.Serialization;

namespace PruebaTecnicaConfuturo.Models.External;

public sealed class IpGeolocationResponse
{
    [JsonPropertyName("city")]
    public string? City { get; init; }

    [JsonPropertyName("state_prov")]
    public string? State { get; init; }

    [JsonPropertyName("country_name")]
    public string? Country { get; init; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; init; }
}
