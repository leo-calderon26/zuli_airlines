namespace zuli_Business.DTO
{
    public class PassengerBreakdownDTO
    {
        public string FullName { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public List<BaggageBreakdownItemDTO> CheckedBags { get; set; } = new();
        public int CarryOnQuantity { get; set; }
        public decimal CarryOnTotal { get; set; }
        public decimal PassengerTotal { get; set; }
    }
}
