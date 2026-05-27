namespace zuli_Business.DTO
{
    public class PurchaseConfirmationPassengerDTO
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PassportCountry { get; set; } = string.Empty;
        public int CheckedBaggageQuantity { get; set; }
        public int CarryOnQuantity { get; set; }
    }
}