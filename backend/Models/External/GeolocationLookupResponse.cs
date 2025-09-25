using System.Text.Json.Serialization;

namespace PruebaTecnicaConfuturo.Models.External;

public sealed class GeolocationLookupResponse
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("state_prov")]
    public string? RegionAlternative { get; set; }

    [JsonPropertyName("regionName")]
    public string? RegionName { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("lat")]
    public double? LatitudeAlternative { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("lon")]
    public double? LongitudeAlternative { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public double? ResolveLatitude() => Latitude ?? LatitudeAlternative;

    public double? ResolveLongitude() => Longitude ?? LongitudeAlternative;

    public string? ResolveRegion() => Region ?? RegionAlternative ?? RegionName;

    public string? ResolveCountry() => Country ?? CountryName;
}
