using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO.External;

namespace zuli_Buisiness.Interface
{
    public interface IExternalAuthorizationService
    {
        Task<AuthorizationResponseDTO> ValidateUser(AuthorizationDTO user);
    }
}
