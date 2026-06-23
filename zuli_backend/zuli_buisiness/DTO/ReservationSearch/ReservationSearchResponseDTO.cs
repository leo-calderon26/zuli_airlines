using System;
using System.Collections.Generic;

namespace zuli_Business.DTO.ReservationSearch
{
    public class ReservationSearchResponseDTO
    {
        public string ReservationCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationCode { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public int PassengerCount { get; set; }
        public ReservationSearchJourneyDTO Journey { get; set; } = new();
    }

    public class ReservationSearchJourneyDTO
    {
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string OriginCity { get; set; } = string.Empty;
        public string OriginCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationCode { get; set; } = string.Empty;
        public string FlightClass { get; set; } = string.Empty;
        
        public int Stops { get; set; }
        public int TotalDurationMinutes { get; set; }

        public List<ReservationSearchLayoverDTO> Layovers { get; set; } = new(); 
        public List<ReservationSearchSegmentDTO> Segments { get; set; } = new();
    }

    public class ReservationSearchLayoverDTO
    {
        public string AirportCode { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }

    public class ReservationSearchSegmentDTO
    {
        public string Airline { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string AircraftModel { get; set; } = string.Empty;
    }
}