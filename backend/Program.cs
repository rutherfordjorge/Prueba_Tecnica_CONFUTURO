using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Services;
using PruebaTecnicaConfuturo.Models;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IGeolocationService, GeolocationService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // puerto de Vite/React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ⚠️ CORS debe ir ANTES que HTTPS y endpoints
app.UseCors("AllowReact");

app.UseHttpsRedirection();

// GET /weather
app.MapGet("/weather", async (IWeatherService weatherService) =>
{
    var forecast = await weatherService.GetForecastAsync();
    return Results.Ok(forecast);
});

app.Run();
