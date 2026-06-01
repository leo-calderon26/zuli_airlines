namespace zuli_Data.Entities
{
    public class PurchaseConfirmationEntity
    {
        public Guid ReservationId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;

        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
        public string FlightClass { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public List<PurchaseConfirmationPassengerEntity> Passengers { get; set; } = new();
        public List<PurchaseConfirmationFlightEntity> Flights { get; set; } = new();
    }
}