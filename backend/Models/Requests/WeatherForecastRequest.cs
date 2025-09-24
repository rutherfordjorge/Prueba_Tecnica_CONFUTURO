using System.ComponentModel.DataAnnotations;

namespace PruebaTecnicaConfuturo.Models.Requests;

public sealed class WeatherForecastRequest
{
    [Range(-90, 90)]
    public double? Latitude { get; init; }

    [Range(-180, 180)]
    public double? Longitude { get; init; }

    public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;
}
