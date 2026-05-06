using System;

namespace zuli_Data.Entities
{
    public class FlightEntity
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime FlightDate { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public DateTime? RealDepartureTime { get; set; }
        public DateTime? RealArrivalTime { get; set; }
        public DateTime? CheckInStartTime { get; set; }
        public DateTime? CheckInDeadline { get; set; }
        public int AirlineId { get; set; }
        public Guid AircraftId { get; set; }
        public int ItineraryId { get; set; }
        public int Duration { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal? CheckedPrice { get; set; }
        public int AvailableSeats { get; set; }
        public Guid AdminId { get; set; }
        public int FlightRouteId { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool Wifi { get; set; }
        public bool Entertainment { get; set; }
        public bool Food { get; set; }
        public bool SeatSelection { get; set; }
    }
}
