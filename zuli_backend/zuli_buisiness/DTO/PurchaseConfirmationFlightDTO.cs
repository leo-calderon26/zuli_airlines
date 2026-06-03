namespace zuli_Business.DTO
{
    public class PurchaseConfirmationFlightDTO
    {
        public Guid FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty;

        public string OriginAirportName { get; set; } = string.Empty;
        public string OriginAirportCode { get; set; } = string.Empty;

        public string DestinationAirportName { get; set; } = string.Empty;
        public string DestinationAirportCode { get; set; } = string.Empty;

        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal? CheckedPrice { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal CheckedBagMultiplier { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }
}