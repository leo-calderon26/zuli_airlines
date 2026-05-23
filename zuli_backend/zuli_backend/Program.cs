using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;
using zuli_backend.Middleware;

using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Business.Validation.Strategies;
using zuli_Data;
using zuli_Repository;
using zuli_Business.Utils;
using AutoMapper;
using zuli_Repository.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");

var secretKey = builder.Configuration["settings:secretkey"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("Secret key not found in configuration");
}
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

// Autenticación existente con JWT + autenticación por cookies para login web
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
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
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "zuli_auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = false;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS para permitir comunicación con Vue/Vite
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Rate limiter para proteger login contra muchos intentos rápidos
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginLimiter", context =>
    {
        string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }
        );
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Registrar DapperContext para manejo de conexiones SQL
builder.Services.AddScoped<DapperContext>();

// Servicios y repositorios existentes
builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();

builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

builder.Services.AddScoped<IExternalFlightService, ExternalFlightService>();
builder.Services.AddScoped<IExternalFlightRepository, ExternalFlightRepository>();

builder.Services.AddScoped<IExternalAuthorizationService, ExternalAuthorizationService>();

builder.Services.AddScoped<IFlightRouteService, FlightRouteService>();
builder.Services.AddScoped<IFlightRouteRepository, FlightRouteRepository>();

// Servicios y repositorios de login
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddSingleton<LoginValidator>();
builder.Services.AddSingleton<RegisterUserValidator>();
builder.Services.AddSingleton<ActivateAccountValidator>();

// FlightRoute strategies
builder.Services.AddScoped<IValidationStrategy<FlightRouteDTO>, FlightRouteBusinessIdStrategy>();
builder.Services.AddScoped<IValidationStrategy<FlightRouteDTO>, FlightRouteAirportExistenceStrategy>();
builder.Services.AddScoped<IValidator<FlightRouteDTO>, FlightRouteValidator>();

// AutoMapper
builder.Services.AddSingleton<IMapper>(sp =>
{
    var configExpression = new MapperConfigurationExpression();
    configExpression.AddProfile<MappingProfile>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var config = new MapperConfiguration(configExpression, loggerFactory);
    return config.CreateMapper();
});

var app = builder.Build();

app.UseGlobalExeption();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseRateLimiter();

app.UseAuthentication();

// app.UseAuthorization();

app.UseMiddleware<LoginValidationMiddleware>();

app.MapControllers();

app.Run();