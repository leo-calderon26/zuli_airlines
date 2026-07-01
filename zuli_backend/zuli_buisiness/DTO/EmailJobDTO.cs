namespace zuli_Business.DTO
{
    public enum EmailJobType
    {
        PurchaseConfirmation = 1,
        AdditionalBaggagePurchase = 2
    }

    public class EmailJobDTO
    {
        public EmailJobType Type { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public int AdditionalCheckedBaggage { get; set; }
        public int AdditionalCarryOn { get; set; }
        public decimal AdditionalBaggageTotal { get; set; }
        public decimal ReservationTotal { get; set; }
    }
}
