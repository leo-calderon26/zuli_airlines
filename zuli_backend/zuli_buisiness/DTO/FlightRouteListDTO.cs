namespace zuli_Business.DTO
{
    public class FlightRouteListDTO
    {
        public int FlightRouteId { get; set; }
        public int Frequency { get; set; }
        public TimeSpan ScheduledArrivalTime { get; set; }
        public TimeSpan ScheduledDepartureTime { get; set; }
        public int EstimatedDuration { get; set; }
        public Guid AdminId { get; set; }
        public int AirlineId { get; set; }
        public string ArrivalAirport { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public string AircraftId { get; set; } = string.Empty;
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedPrice { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public decimal CheckedBagMultiplier { get; set; }
        public decimal MaxWeightPerBag { get; set; }
    }
}