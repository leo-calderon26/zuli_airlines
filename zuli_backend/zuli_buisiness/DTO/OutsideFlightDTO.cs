using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace zuli_Business.DTO
{
    public class OutsideFlightResponseDTO
    {
        [JsonPropertyName("flights")]
        public List<OutsideFlightDTO> OutsideFlights { get; set; } = new();
    }
    public class OutsideAirportDTO
    {
        public string code { get; set; }
        public string name { get; set; }
        public string city { get; set; }
    }
    public class OutsideFlightDTO
    {
        public int AirlineId { get; set; }
        [JsonPropertyName("flightGUID")]
        public string Id { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        [JsonPropertyName("departureTime")]
        public DateTime DepartureDateTime { get; set; }
        [JsonPropertyName("arrivalTime")]
        public DateTime ArrivalDateTime { get; set; }
        public string Duration { get; set; }
        public int DurationOnSeconds { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal? CheckedPrice { get; set; }
        [JsonPropertyName("arrivalAirport")]
        public OutsideAirportDTO RealArrivalAirport { get; set; }
        [JsonPropertyName("departureAirport")]
        public OutsideAirportDTO RealDepartureAirport { get; set; }
        public int Frequency { get; set; }
    }
}