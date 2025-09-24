namespace PruebaTecnicaConfuturo.Domain.ValueObjects;

public readonly record struct Temperature(double Celsius)
{
    public double Fahrenheit => Math.Round((Celsius * 9 / 5) + 32, 1);
    public double Kelvin => Math.Round(Celsius + 273.15, 2);
}
