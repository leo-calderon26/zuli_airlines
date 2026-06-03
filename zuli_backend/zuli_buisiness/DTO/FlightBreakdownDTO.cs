namespace zuli_Business.DTO
{
    public class FlightBreakdownDTO
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;
        public List<PassengerBreakdownDTO> Passengers { get; set; } = new();
        public decimal FlightTotal { get; set; }
    }
}
