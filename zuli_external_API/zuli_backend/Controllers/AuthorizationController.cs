using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.External;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalAuthorizationController : ControllerBase
    {
        private readonly IExternalAuthorizationService _service;
        public ExternalAuthorizationController(IExternalAuthorizationService service) => _service = service;

        [HttpPost]
        [Route("Validar")]
        public async Task<ActionResult<AuthorizationResponseDTO>> ValidateUser([FromBody] AuthorizationDTO user)
            => await _service.ValidateUser(user);
    }
}
