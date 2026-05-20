using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;

namespace zuli_Business.Interface
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponseDTO> ValidateUser(AuthorizationDTO user);
    }
}
