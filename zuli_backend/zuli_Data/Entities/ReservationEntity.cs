namespace zuli_Data.Entities
{
    public class ReservationEntity
    {
        public int ReservationId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public string? ReservationOrigin { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int BuyerId { get; set; }
        public string? FlightClass { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
