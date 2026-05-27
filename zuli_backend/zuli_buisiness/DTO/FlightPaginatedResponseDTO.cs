using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class FlightPaginatedResponseDTO
    {
        public int TotalRecordsDeparture { get; set; }
        public int TotalPagesDeparture { get; set; }
        public List<FlightSearchResponseDTO> DepartureFlights { get; set; } = new List<FlightSearchResponseDTO>();

        public int TotalRecordsReturn { get; set; }
        public int TotalPagesReturn { get; set; }
        public List<FlightSearchResponseDTO> ReturnFlights { get; set; } = new List<FlightSearchResponseDTO>();
        public int CurrentPage { get; set; }
    }
}