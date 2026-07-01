namespace zuli_Data.Entities
{
    public class ReservationSearchPassengerEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string SecondLastName { get; set; } = string.Empty;
        public int CheckedBaggageQuantity { get; set; }
        public int CarryOnQuantity { get; set; }
    }
}