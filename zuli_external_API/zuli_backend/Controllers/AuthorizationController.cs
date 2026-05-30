using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _service;
        public AuthorizationController(IAuthorizationService service) => _service = service;

        [HttpPost]
        [Route("Validar")]
        public async Task<ActionResult<AuthorizationResponseDTO>> ValidateUser([FromBody] AuthorizationDTO user)
            => await _service.ValidateUser(user);
    }
}
