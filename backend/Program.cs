using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.Options;
using PruebaTecnicaConfuturo.Models.Requests;
using PruebaTecnicaConfuturo.Services;
using PruebaTecnicaConfuturo.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("ExternalApis:Weather"));
builder.Services.Configure<GeolocationApiOptions>(builder.Configuration.GetSection("ExternalApis:Geolocation"));

builder.Services.AddHttpClient(WeatherService.HttpClientName, (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        client.BaseAddress = new Uri(options.BaseUrl);
    }
});

builder.Services.AddHttpClient(GeolocationService.HttpClientName, (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<GeolocationApiOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        client.BaseAddress = new Uri(options.BaseUrl);
    }
});

builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IGeolocationService, GeolocationService>();

builder.Services.AddScoped<IValidator<WeatherForecastRequest>, WeatherForecastRequestValidator>();

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
