using PruebaTecnicaConfuturo.Interfaces;

namespace PruebaTecnicaConfuturo.Services
{
    public class GeolocationService : IGeolocationService
    {
        public string GetCurrentLocation()
        {
            // Simulación de geolocalización estática
            return "Santiago, Chile";
        }
    }
}
