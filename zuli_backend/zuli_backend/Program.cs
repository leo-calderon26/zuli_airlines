using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;
using zuli_backend.Middleware;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Interface.Reports;
using zuli_Business.Mappings;
using zuli_Business.Reports;
using zuli_Business.Utils;
using zuli_Business.Validation;
using zuli_Business.Validation.Strategies;
using zuli_Data;
using zuli_Repository;
using zuli_Repository.Interface;
using zuli_Repository.Interface.Reports;
using zuli_Repository.Reports;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var secretKey = builder.Configuration["settings:secretKey"];
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

builder.Services.AddHttpClient();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS para permitir comunicación con Vue/Vite y el proxy nginx en producción
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        // Permitir cualquier origen con credenciales.
        // En producción el backend está protegido por el proxy nginx (mismo origen),
        // por lo que CORS solo aplica para desarrollo local o acceso directo.
        policy.SetIsOriginAllowed(_ => true)
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

builder.Services.Configure<List<ExternalAirlinesDTO>>(
    builder.Configuration.GetSection("ExternalAirlines"));

// Registrar DapperContext para manejo de conexiones SQL
builder.Services.AddScoped<DapperContext>();

// Servicios y repositorios existentes
builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();

builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();

builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

builder.Services.AddScoped<IFlightRouteService, FlightRouteService>();
builder.Services.AddScoped<IFlightRouteRepository, FlightRouteRepository>();

// Búsqueda de vuelos con otras aerolíneas
builder.Services.AddScoped<IOutsideFlightService, OutsideFlightService>();
builder.Services.AddScoped<IOutsideFlightRepository, OutsideFlightRepository>();
builder.Services.AddScoped<FluentValidation.IValidator<OutsideFlightRequestDTO>, OutsideFlightSearchValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<OutsideFlightDTO>, OutsideFlightValidator>();

// Servicios y repositorios de login
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<ITicketPurchaseService, TicketPurchaseService>();
builder.Services.AddScoped<IReservationCreationService, ReservationCreationService>();
builder.Services.AddScoped<IFlightResolverService, FlightResolverService>();
builder.Services.AddScoped<IPassengerValidationService, PassengerValidationService>();
builder.Services.AddScoped<IPassengerCreationService, PassengerCreationService>();
builder.Services.AddScoped<IBaggageRegistrationService, BaggageRegistrationService>();
builder.Services.AddValidatorsFromAssemblyContaining<TicketPurchaseRequestValidator>();

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IBaggageRepository, BaggageRepository>();

builder.Services.AddScoped<IPurchaseConfirmationRepository, PurchaseConfirmationRepository>();
builder.Services.AddScoped<IPurchaseConfirmationService, PurchaseConfirmationService>();
builder.Services.AddScoped<IPurchaseConfirmationPdfService, PurchaseConfirmationPdfService>();
builder.Services.AddScoped<FluentValidation.IValidator<PurchaseConfirmationPageDTO>, PurchaseConfirmationValidator>();

builder.Services.AddSingleton<LoginValidator>();
builder.Services.AddSingleton<RegisterUserValidator>();
builder.Services.AddSingleton<ActivateAccountValidator>();

builder.Services.AddScoped<IReservationSearchRepository, ReservationSearchRepository>();
builder.Services.AddScoped<IReservationSearchService, ReservationSearchService>();

builder.Services.AddScoped<IReservationCancellationRepository, ReservationCancellationRepository>();
builder.Services.AddScoped<IReservationCancellationService, ReservationCancellationService>();

// FlightRoute strategies
builder.Services.AddScoped<IValidationStrategy<FlightRouteDTO>, FlightRouteBusinessIdStrategy>();
builder.Services.AddScoped<IValidationStrategy<FlightRouteDTO>, FlightRouteAirportExistenceStrategy>();
builder.Services.AddScoped<zuli_Business.Validation.IValidator<FlightRouteDTO>, FlightRouteValidator>();

// Flight search utils
builder.Services.AddScoped<IFlightPathFinder, FlightPathFinder>();

builder.Services.AddScoped<IFilterOptionsService, FilterOptionsService>();
builder.Services.AddScoped<IFilterOptionsRepository, FilterOptionsRepository>();
builder.Services.AddScoped<IIncomeReportService, IncomeReportService>();
builder.Services.AddScoped<IIncomeReportExportService, IncomeReportExportService>();
builder.Services.AddScoped<IIncomeReportRepository, IncomeReportRepository>();

var config = TypeAdapterConfig.GlobalSettings;
config.Scan(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddValidatorsFromAssemblyContaining<zuli_Business.Validation.FlightValidator>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

builder.Services.AddScoped<IFlightsReportRepository, FlightsReportRepository>();
builder.Services.AddScoped<IFlightsReportService, FlightsReportService>();
builder.Services.AddScoped<IFlightsReportExportService, FlightsReportExportService>();

builder.Services.AddScoped<IFlightsReportRepository, FlightsReportRepository>();
builder.Services.AddScoped<IFlightsReportService, FlightsReportService>();
builder.Services.AddScoped<IFlightsReportExportService, FlightsReportExportService>();

builder.Services.AddScoped<IFlightsReportRepository, FlightsReportRepository>();
builder.Services.AddScoped<IFlightsReportService, FlightsReportService>();
builder.Services.AddScoped<IFlightsReportExportService, FlightsReportExportService>();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

app.UseGlobalExeption();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("FrontendPolicy");

app.UseRateLimiter();

app.UseAuthentication();

app.UseMiddleware<LoginValidationMiddleware>();

app.MapControllers();

app.Run();