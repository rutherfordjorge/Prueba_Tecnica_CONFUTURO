using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaConfuturo.Domain.Entities;
using PruebaTecnicaConfuturo.Domain.ValueObjects;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.Dtos;
using PruebaTecnicaConfuturo.Models.Requests;

namespace PruebaTecnicaConfuturo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IValidator<WeatherForecastRequest> _validator;
    private readonly IMapper _mapper;

    public WeatherController(
        IWeatherService weatherService,
        IValidator<WeatherForecastRequest> validator,
        IMapper mapper)
    {
        _weatherService = weatherService;
        _validator = validator;
        _mapper = mapper;
    }

    [HttpGet("forecast")]
    [ProducesResponseType(typeof(ForecastResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetForecastAsync([FromQuery] WeatherForecastRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var location = request.HasCoordinates
            ? new Location
            {
                City = "Ubicación personalizada",
                Region = null,
                Country = string.Empty,
                Coordinates = new Coordinates(request.Latitude!.Value, request.Longitude!.Value)
            }
            : Location.CreateFallback();

        var report = await _weatherService.GetForecastAsync(location, cancellationToken);
        var dto = _mapper.Map<ForecastResponseDto>(report);
        return Ok(dto);
    }
}
