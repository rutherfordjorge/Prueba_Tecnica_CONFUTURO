using FluentValidation;
using Microsoft.Extensions.Options;
using Refit;
using System.Text.Json;
using PruebaTecnicaConfuturo.Interfaces;
using PruebaTecnicaConfuturo.Models.Options;
using PruebaTecnicaConfuturo.Models.Requests;
using PruebaTecnicaConfuturo.Services;
using PruebaTecnicaConfuturo.Services.Clients;
using PruebaTecnicaConfuturo.Validators;
using PruebaTecnicaConfuturo.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("ExternalApis:Weather"));
builder.Services.Configure<GeolocationApiOptions>(builder.Configuration.GetSection("ExternalApis:Geolocation"));

builder.Services.AddHttpClient(WeatherService.HttpClientName, (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(options.ForecastBaseUrl))
    {
        client.BaseAddress = new Uri(options.ForecastBaseUrl);
    }
});

builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services
    .AddRefitClient<IIpGeolocationApi>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<GeolocationApiOptions>>().Value;
        var baseUrl = options.BaseUrl ?? "https://api.ipgeolocation.io/";
        client.BaseAddress = new Uri(baseUrl);
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IValidator<WeatherForecastRequest>, WeatherForecastRequestValidator>();
builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
