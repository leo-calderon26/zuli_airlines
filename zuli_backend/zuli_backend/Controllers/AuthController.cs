using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("LoginLimiter")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            AuthResultDTO result = await authService.LoginAsync(request);

            if (!result.Success || result.User == null)
            {
                return Unauthorized(result.Response);
            }

            await SignInUserAsync(result.User);

            return Ok(result.Response);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public IActionResult Me()
        {
            LoginResponseDTO response = authService.BuildAuthenticatedUserResponse(
                User.FindFirstValue(ClaimTypes.Email),
                User.FindFirstValue("BusinessId"),
                User.FindFirstValue(ClaimTypes.Role)
            );

            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            LoginResponseDTO response = authService.BuildLogoutResponse();

            return Ok(response);
        }

        private async Task SignInUserAsync(AppUser user)
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.BusinessEmail),
                new Claim("BusinessId", user.BusinessId),
                new Claim(ClaimTypes.Role, user.UserRole)
            ];

            ClaimsIdentity identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            AuthenticationProperties properties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties
            );
        }
    }
}