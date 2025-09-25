namespace PruebaTecnicaConfuturo.Models.Options;

public sealed class GeolocationApiOptions
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }

    /// <summary>
    ///     Nombre del parámetro de query que utiliza la API externa para recibir la API key.
    ///     Permite compatibilizar distintos proveedores sin cambiar el código fuente.
    /// </summary>
    public string ApiKeyQueryParameter { get; set; } = "apiKey";

    public bool HasValidConfiguration => !string.IsNullOrWhiteSpace(BaseUrl);
}
