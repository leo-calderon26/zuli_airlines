namespace zuli_Data.Entities
{
    public class BaggageEntity
    {
        public int BaggageId { get; set; }
        public int PassengerId { get; set; }
        public int ReservationId { get; set; }
        public decimal Weight { get; set; }
        public string Size { get; set; } = string.Empty;
        public string? Type { get; set; }
    }
}
