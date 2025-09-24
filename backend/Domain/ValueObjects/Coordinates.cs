namespace PruebaTecnicaConfuturo.Domain.ValueObjects;

public readonly record struct Coordinates(double Latitude, double Longitude)
{
    public bool IsValid => Latitude is >= -90 and <= 90 && Longitude is >= -180 and <= 180;
}
