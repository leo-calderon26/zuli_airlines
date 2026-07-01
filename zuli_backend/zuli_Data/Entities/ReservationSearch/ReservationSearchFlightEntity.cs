namespace zuli_Data.Entities
{
    public class ReservationSearchFlightEntity
    {
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string OriginCity { get; set; } = string.Empty;
        public string OriginCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationCode { get; set; } = string.Empty;
        public string FlightClass { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string AircraftModel { get; set; } = string.Empty;
        public int ReservationStatusId { get; set; }
        public string BuyerEmail { get; set; } = string.Empty;
    }
}