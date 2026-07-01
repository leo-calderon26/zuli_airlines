namespace zuli_Data.Entities
{
    public class PurchaseConfirmationPassengerEntity
    {
        public int PassengerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PassportCountry { get; set; } = string.Empty;
        public int CheckedBaggageQuantity { get; set; }
        public int CarryOnQuantity { get; set; }
    }
}