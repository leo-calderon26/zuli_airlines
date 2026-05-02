using System;

namespace zuli_Data.Entities
{
    public class FlightEntity
    {
        public Guid FlightId { get; set; }
        public Guid aircraftId { get; set; }
        public string originAirport { get; set; } = string.Empty;
        public string destinationAirport { get; set; } = string.Empty;
        public bool monday { get; set; }
        public bool tuesday { get; set; }
        public bool wednesday { get; set; }
        public bool thursday { get; set; }
        public bool friday { get; set; }
        public bool saturday { get; set; }
        public bool sunday { get; set; }
        public TimeSpan departureTime { get; set; }
        public TimeSpan arrivalTime { get; set; }
        public TimeSpan duration { get; set; }
        public decimal firstClassPrice { get; set; }
        public decimal touristPrice { get; set; }
        public decimal carryOnPrice { get; set; }
        public decimal carryOnWeightKg { get; set; }
        public decimal checkedBaggagePrice { get; set; }
        public decimal checkedBaggageMaxWeightKg { get; set; }
        public decimal checkedBaggageMultiplierPercent { get; set; }
        public int availableSeats { get; set; }
        public string? status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
