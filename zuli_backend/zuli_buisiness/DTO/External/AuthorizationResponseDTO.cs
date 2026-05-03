using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace zuli_Business.DTO.External
{
    public class AuthorizationResponseDTO
    {
        public int StatusCode { get; set; }
        public string CreatedToken { get; set; } = string.Empty;
    }
}
