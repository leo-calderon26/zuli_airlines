using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace zuli_Business.DTO
{
    public class AirportDTO
    {
        public string code { get; set; }
        public string name { get; set; }
        public string city { get; set; }
    }
    public class FlightBreakupDTO {
        public Guid FlightGUID { get; set; }
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public AirportDTO ArrivalAirport { get; set; }
        public AirportDTO DepartureAirport { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal? CheckedPrice { get; set; }
    }
    public class PaymentBreakupDTO {
        public decimal Luggage { get; set; }
        public decimal Tickets { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
    }
    public class BuyerBreakupDTO {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string LastName2 { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class ReservationResponseDTO
    {
        public string reservationNumber { get; set; }
        public bool firstClass { get; set; } = false;
        public FlightBreakupDTO flightBreakup { get; set; }
        public PaymentBreakupDTO paymentInfo { get; set; }
        public List<PassengerInfoDTO> passengersInfoDTO { get; set; }
        public BuyerBreakupDTO buyerInfo { get; set; }
    }
}
