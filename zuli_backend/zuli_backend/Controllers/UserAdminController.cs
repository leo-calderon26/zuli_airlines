using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(
        AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
        Roles = "Administrator"
    )]
    public class UserAdminController : ControllerBase
    {
        private readonly IUserRegistrationService _userRegistrationService;

        public UserAdminController(IUserRegistrationService userRegistrationService)
        {
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDTO request)
        {
            string? adminUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(adminUserIdValue, out Guid adminUserId))
            {
                return Unauthorized(new BasicResponseDTO
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "No se pudo identificar al administrador."
                });
            }

            RegisterUserResponseDTO response = await _userRegistrationService.RegisterUserAsync(
                request,
                adminUserId
            );

            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? searchType,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            UserSearchResponseDTO response = await _userRegistrationService.GetUsersAsync(
                searchType,
                search,
                page,
                pageSize
            );
            
            return Ok(response);
}
    }
}