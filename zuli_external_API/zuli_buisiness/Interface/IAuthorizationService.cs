using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;

namespace zuli_Buisiness.Interface
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponseDTO> ValidateUser(AuthorizationDTO user);
    }
}
