namespace zuli_Business.DTO
{
    public class BaggageBreakdownItemDTO
    {
        public int BagNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
