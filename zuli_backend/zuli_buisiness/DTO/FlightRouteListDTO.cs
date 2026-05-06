namespace zuli_Business.DTO
{
    public class FlightRouteListDTO
    {
        public int FlightRouteId { get; set; }
        public int Frequency { get; set; }
        public DateTime ScheduledArrivalTime { get; set; }
        public DateTime ScheduledDepartureTime { get; set; }
        public int EstimatedDuration { get; set; }
        public Guid AdminId { get; set; }
        public int AirlineId { get; set; }
        public string ArrivalAirport { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
    }
}