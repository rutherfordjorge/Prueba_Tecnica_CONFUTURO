using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaTecnicaConfuturo.Services
{
    public class WeatherService : IWeatherService
    {
        public async Task<IEnumerable<WeatherForecast>> GetForecastAsync()
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild",
                "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                }).ToList();

            return await Task.FromResult(forecast);
        }
    }
}
