using System.Text;
using System.Text.Json;
using zuli_Business.DTO;
using zuli_Business.Validation;
using zuli_Data.Exceptions;

namespace zuli_backend.Middleware
{
    public class LoginValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly LoginValidator loginValidator;

        public LoginValidationMiddleware(RequestDelegate next, LoginValidator loginValidator)
        {
            this.next = next;
            this.loginValidator = loginValidator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsLoginRequest(context))
            {
                await next(context);
                return;
            }

            context.Request.EnableBuffering();

            using StreamReader reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true
            );

            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            LoginRequestDTO? request;

            try
            {
                request = JsonSerializer.Deserialize<LoginRequestDTO>(
                    body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
            }
            catch (JsonException)
            {
                Dictionary<string, string[]> errors = new()
                {
                    ["request"] = ["Solicitud inválida."]
                };

                throw new ZuliValidationException(errors);
            }

            loginValidator.Validate(request);

            await next(context);
        }

        private static bool IsLoginRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/api/auth/login")
                && context.Request.Method == HttpMethods.Post;
        }
    }
}