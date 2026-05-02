using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IAircraftService
    {
        Task<BasicResponseDTO> CreateAircraft(AircraftDTO aircraft);
        Task<IEnumerable<AircraftDTO>> GetAll();
        Task<PaginatedResponseDTO<AircraftDTO>> GetAircraftsPaginated(int pageNumber, int pageSize);
    }
}
