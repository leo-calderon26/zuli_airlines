
using zuli_Buisiness;
using zuli_Data.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;

namespace zuli_backend.Middleware
{
    public class GlobalExeptionMiddleware
    {
        // Esta variable se usa para pasar al siguente middleware sino esto se queda pegado
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExeptionMiddleware> _logger;

        // TODO(you) hacer que los errores se guarden en unos logs si es que los profes lo piden
        // recuerde que el middleware es un singleton
        public GlobalExeptionMiddleware(RequestDelegate next, ILogger<GlobalExeptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch(AppExeptions exeption)
            {
                _logger.LogWarning(exeption, "App exception: {Code}", exeption.ErrorCode);
                await WriteProblemAsync(
                    context,
                    exeption.StatusCode,
                    exeption.ErrorCode,
                    exeption.Message,
                    exeption,
                    exeption.ErrorId
                    );
            }
            catch(Exception exeption)
            {
                // Error no controlado
                _logger.LogError(exeption, "unhandled error");
                // TODO(you) Aqui se debe de crear una forma para guardar los logs del error de 
                // forma que siga funcionando
                await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, 
                    "INTERNAL_ERROR", "An unexpected error occurred.");
            }
        }
        private async Task WriteProblemAsync(HttpContext context, int statusCode, 
            string errorCode, string detail, AppExeptions? appEx = null, int ErrorId = -1)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode =statusCode;
            var error = new ProblemDetails
            {
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = detail,
                Instance = context.Request.Path
            };
            error.Extensions["errorCode"] = errorCode;
            error.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

            if (appEx is ZuliValidationException ve)
                error.Extensions["errors"] = ve.Errors;

            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
