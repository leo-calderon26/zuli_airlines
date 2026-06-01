namespace zuli_Data.Entities
{
    public class PurchaseConfirmationFlightEntity
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
    }
}