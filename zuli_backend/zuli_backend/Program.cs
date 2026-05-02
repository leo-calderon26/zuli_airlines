using Scalar.AspNetCore;
using zuli_backend.Middleware;
using zuli_Buisiness;
using zuli_Buisiness.Interface;
using zuli_Data;
using zuli_Repository;
using zuli_Repository.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Registrar DapperContext para manejo de conexiones SQL
builder.Services.AddScoped<DapperContext>();


builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();

builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();

builder.Services.AddScoped<IFlightRouteService, FlightRouteService>();
builder.Services.AddScoped<IFlightRouteRepository, FlightRouteRepository>();

var app = builder.Build();
app.UseGlobalExeption();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
