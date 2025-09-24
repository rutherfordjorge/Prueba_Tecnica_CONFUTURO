using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebaTecnicaConfuturo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IGeolocationService _geolocationService;

        public WeatherController(IWeatherService weatherService, IGeolocationService geolocationService)
        {
            _weatherService = weatherService;
            _geolocationService = geolocationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            var location = _geolocationService.GetCurrentLocation();
            var forecast = await _weatherService.GetForecastAsync();

            return Ok(new
            {
                Location = location,
                Forecast = forecast
            });
        }
    }
}
