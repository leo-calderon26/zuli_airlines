using System.Collections.Generic;

namespace zuli_Business.DTO
{
    public class PaginatedFlightListDTO
    {
        public List<FlightSearchResponseDTO> Flights { get; set; } = new List<FlightSearchResponseDTO>();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}