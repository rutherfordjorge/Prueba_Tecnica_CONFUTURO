using PruebaTecnicaConfuturo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebaTecnicaConfuturo.Interfaces
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetForecastAsync();
    }
}
