using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountActivationController : ControllerBase
    {
        private readonly IUserRegistrationService _userRegistrationService;

        public AccountActivationController(IUserRegistrationService userRegistrationService)
        {
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountRequestDTO request)
        {
            BasicResponseDTO response = await _userRegistrationService.ActivateAccountAsync(request);

            return Ok(response);
        }
    }
}