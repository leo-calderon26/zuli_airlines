using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using zuli_Business;
using zuli_Business.Interface;
using zuli_Business.Utils;
using zuli_Data;
using zuli_external_API.Middleware;
using zuli_Repository;
using zuli_Repository.Interface;
using Mapster;
using MapsterMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("settings").GetSection("secretkey").ToString();
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthorization().AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
    config =>
    {
        config.RequireHttpsMetadata = false;
        config.SaveToken = true;
        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

TypeAdapterConfig<zuli_Data.Entities.RawFlightEntity, zuli_Business.DTO.BookedFlightDTO>
    .NewConfig()
    .Map(d => d.flightGUID, s => s.FlightGUID)
    .Map(d => d.departureAirportCode, s => s.DepartureAiportCode)
    .Map(d => d.arrivalAirportCode, s => s.ArrivalAiportCode)
    .Map(d => d.duration, s => s.EstimatedDuration)
    .Map(d => d.departureTime, s => s.DepartureTime)
    .Map(d => d.arrivalTime, s => s.ArrivalTime)
    .Map(d => d.departureAirportName, s => s.DepartureAiportName)
    .Map(d => d.arrivalAirportName, s => s.ArrivalAirportName)
    .Map(d => d.departureAirportCity, s => s.DepartureCityName)
    .Map(d => d.arrivalAirportCity, s => s.ArrivalAirportCity)
    .Map(d => d.touristPrice, s => s.TouristPrice)
    .Map(d => d.firstClassPrice, s => s.FirstClassPrice)
    .Map(d => d.carryOnPrice, s => s.CarryOnPrice)
    .Map(d => d.checkedPrice, s => s.CheckedPrice);


// Registrar DapperContext para manejo de conexiones SQL
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightDateGenerator, FlightDateGenerator>();
builder.Services.AddScoped<IFlightPathFinder, FlightPathFinder>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var app = builder.Build();
app.UseGlobalExeption();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
