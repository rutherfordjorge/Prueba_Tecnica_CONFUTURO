using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.Dtos;

namespace PruebaTecnicaConfuturo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocationController : ControllerBase
{
    private readonly IGeolocationService _geolocationService;

    public LocationController(IGeolocationService geolocationService)
    {
        _geolocationService = geolocationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var location = await _geolocationService.ResolveCurrentLocationAsync(cancellationToken);
        return Ok(LocationResponseDto.FromDomain(location));
    }
}
