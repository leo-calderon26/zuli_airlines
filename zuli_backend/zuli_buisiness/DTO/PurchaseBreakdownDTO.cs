namespace zuli_Business.DTO
{
    public class PurchaseBreakdownDTO
    {
        public List<FlightBreakdownDTO> Flights { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }
}
