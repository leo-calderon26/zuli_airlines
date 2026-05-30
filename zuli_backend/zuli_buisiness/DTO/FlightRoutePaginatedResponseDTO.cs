using System.Collections.Generic;

namespace zuli_Business.DTO
{
    public class FlightRoutePaginatedResponseDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<FlightRouteListDTO> Data { get; set; } = [];
    }
}