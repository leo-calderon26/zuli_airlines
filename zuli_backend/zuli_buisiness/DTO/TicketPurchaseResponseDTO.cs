namespace zuli_Business.DTO
{
    public class TicketPurchaseResponseDTO
    {
        public string ConfirmationCode { get; set; } = string.Empty;
        public int ReservationId { get; set; }
        public decimal TotalPayment { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
