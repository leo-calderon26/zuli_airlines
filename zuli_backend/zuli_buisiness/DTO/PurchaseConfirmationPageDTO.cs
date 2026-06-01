namespace zuli_Business.DTO
{
    public class PurchaseConfirmationPageDTO
    {
        public int ReservationId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public string Message { get; set; } = "Reserva completada. Los detalles han sido enviados a su correo electrónico.";

        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
        public string FlightClass { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public bool InvoiceEmailSent { get; set; }
        public bool ConfirmationEmailSent { get; set; }

        public bool EmailsSent =>
            InvoiceEmailSent &&
            ConfirmationEmailSent;

        public List<PurchaseConfirmationPassengerDTO> Passengers { get; set; } = new();
        public List<PurchaseConfirmationFlightDTO> Flights { get; set; } = new();
    }
}