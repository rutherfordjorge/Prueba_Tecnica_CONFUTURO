using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Services;
using PruebaTecnicaConfuturo.Models;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IGeolocationService, GeolocationService>();

var app = builder.Build();

app.UseHttpsRedirection();

// GET /weather
app.MapGet("/weather", async (IWeatherService weatherService) =>
{
    var forecast = await weatherService.GetForecastAsync();
    return Results.Ok(forecast);
});

app.Run();
