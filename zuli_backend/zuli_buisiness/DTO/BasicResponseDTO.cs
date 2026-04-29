using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace zuli_Buisiness.DTO
{
    public class BasicResponseDTO
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
