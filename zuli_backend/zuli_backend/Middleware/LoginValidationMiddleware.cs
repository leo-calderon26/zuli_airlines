using System.Text;
using System.Text.Json;
using zuli_buisiness.DTO;
using zuli_buisiness.Validation;

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

			LoginRequestDTO? request = JsonSerializer.Deserialize<LoginRequestDTO>(
				body,
				new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				}
			);

			string? error = loginValidator.Validate(request);

			if (error != null)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				context.Response.ContentType = "application/json";

				await context.Response.WriteAsJsonAsync(new LoginResponseDTO
				{
					Success = false,
					Message = error
				});

				return;
			}

			await next(context);
		}

		private bool IsLoginRequest(HttpContext context)
		{
			return context.Request.Path.StartsWithSegments("/api/auth/login")
				&& context.Request.Method == HttpMethods.Post;
		}
	}
}