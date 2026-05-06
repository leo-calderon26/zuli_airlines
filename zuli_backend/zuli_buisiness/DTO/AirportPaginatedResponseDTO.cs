using System;
using System.Collections.Generic;

namespace zuli_Business.DTO
{
    public class AirportPaginatedResponseDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<AirportDTO> Data { get; set; }
    }
}