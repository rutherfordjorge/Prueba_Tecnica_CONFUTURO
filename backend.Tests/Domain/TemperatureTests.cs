using PruebaTecnicaConfuturo.Domain.ValueObjects;
using Xunit;

namespace PruebaTecnicaConfuturo.Tests.Domain;

public class TemperatureTests
{
    [Fact]
    public void ConvertsCelsiusToFahrenheitAndKelvin()
    {
        // Arrange
        var temperature = new Temperature(25);

        // Act
        var fahrenheit = temperature.Fahrenheit;
        var kelvin = temperature.Kelvin;

        // Assert
        Assert.Equal(77.0, fahrenheit);
        Assert.Equal(298.15, kelvin);
    }
}
