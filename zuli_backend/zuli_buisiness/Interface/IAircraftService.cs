using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;

namespace zuli_Buisiness.Interface
{
    public interface IAircraftService
    {
        Task<BasicResponseDTO> CreateAircraft(AircraftDTO aircraft);
    }
}
