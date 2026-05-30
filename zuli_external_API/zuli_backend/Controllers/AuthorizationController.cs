using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD:zuli_external_API/zuli_backend/Controllers/AuthorizationController.cs
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
=======
using zuli_Business.DTO.External;
using zuli_Business.Interface;
>>>>>>> develop:zuli_backend/zuli_backend/Controllers/ExternalAuthorizationController.cs

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
