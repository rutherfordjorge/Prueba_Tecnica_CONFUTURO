namespace PruebaTecnicaConfuturo.Models.Options;

public sealed class GeolocationApiOptions
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }

    public bool HasValidConfiguration => !string.IsNullOrWhiteSpace(BaseUrl) && !string.IsNullOrWhiteSpace(ApiKey);
}
