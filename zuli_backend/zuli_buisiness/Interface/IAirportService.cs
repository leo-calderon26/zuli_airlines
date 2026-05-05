using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IAirportService
    {
        Task<BasicResponseDTO> CreateAirport(AirportDTO Airport);
        Task<List<AirportSuggestionDTO>> GetAirportSuggestions(string searchTerm);
    }
}
