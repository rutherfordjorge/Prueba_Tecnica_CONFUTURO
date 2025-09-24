using FluentValidation;
using PruebaTecnicaConfuturo.Models.Requests;

namespace PruebaTecnicaConfuturo.Validators;

public sealed class WeatherForecastRequestValidator : AbstractValidator<WeatherForecastRequest>
{
    public WeatherForecastRequestValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue)
            .WithMessage("La latitud debe estar entre -90 y 90 grados.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue)
            .WithMessage("La longitud debe estar entre -180 y 180 grados.");
    }
}
