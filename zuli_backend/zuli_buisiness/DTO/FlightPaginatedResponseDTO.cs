using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class FlightPaginatedResponseDTO
    {
        public int TotalRecordsOutbound { get; set; }
        public int TotalPagesOutbound { get; set; }
        public List<FlightSearchResponseDTO> OutboundFlights { get; set; } = new List<FlightSearchResponseDTO>();

        public int TotalRecordsReturn { get; set; }
        public int TotalPagesReturn { get; set; }
        public List<FlightSearchResponseDTO> ReturnFlights { get; set; } = new List<FlightSearchResponseDTO>();
        public int CurrentPage { get; set; }
    }
}