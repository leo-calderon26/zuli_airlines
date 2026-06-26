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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UserAdminController : ControllerBase
    {
        private readonly IUserRegistrationService _userRegistrationService;

        public UserAdminController(IUserRegistrationService userRegistrationService)
        {
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator,Operator")]
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

        [HttpPut("{userId:guid}")]
        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IActionResult> UpdateUserAsync(Guid userId, [FromBody] RegisterUserRequestDTO request)
        {
            BasicResponseDTO response = await _userRegistrationService.UpdateUserAsync(userId, request);

            return Ok(response);
        }
        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            string? authenticatedUserIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!Guid.TryParse(authenticatedUserIdValue, out Guid authenticatedUserId))
            {
                return Unauthorized(new BasicResponseDTO
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "No se pudo identificar al administrador."
                });
            }

            BasicResponseDTO response = await _userRegistrationService.DeleteUserAsync(
                userId,
                authenticatedUserId
            );

            return Ok(response);
        }
    }
}