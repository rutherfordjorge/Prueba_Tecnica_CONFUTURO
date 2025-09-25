using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.Dtos;

namespace PruebaTecnicaConfuturo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocationController : ControllerBase
{
    private readonly IGeolocationService _geolocationService;
    private readonly IMapper _mapper;

    public LocationController(IGeolocationService geolocationService, IMapper mapper)
    {
        _geolocationService = geolocationService;
        _mapper = mapper;
    }

    [HttpGet("resolve")]
    [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResolveAsync(CancellationToken cancellationToken = default)
    {
        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var location = await _geolocationService.ResolveAsync(remoteIp, cancellationToken);
        var dto = _mapper.Map<LocationResponseDto>(location);
        return Ok(dto);
    }
}
