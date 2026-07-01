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
using zuli_Business.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var secretKey = builder.Configuration["settings:secretKey"];
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

        config.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Query["ApiToken"].ToString();
                return Task.CompletedTask;
            }
        };

        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddHttpClient();
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

builder.Services.Configure<AirlineClientDTO>(
    builder.Configuration.GetSection("AirlineClient"));

// Registrar DapperContext para manejo de conexiones SQL
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightDateGenerator, FlightDateGenerator>();
builder.Services.AddScoped<IFlightPathFinder, FlightPathFinder>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

var config = TypeAdapterConfig.GlobalSettings;
config.Scan(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var app = builder.Build();
app.UseGlobalExeption();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
