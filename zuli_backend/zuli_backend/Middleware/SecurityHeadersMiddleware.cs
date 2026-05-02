namespace zuli_backend.Middleware
{
	public class SecurityHeadersMiddleware
	{
		private readonly RequestDelegate next;

		public SecurityHeadersMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
			context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
			context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
			context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

			await next(context);
		}
	}
}