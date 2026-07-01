namespace zuli_Business.DTO
{
    public class ReservationResponseMappingContextDTO
    {
        public ReservationRequestDTO ReservationRequest { get; set; } = new();
        public ReservedFlightDTO ReservedFlightData { get; set; } = new();
        public TicketPurchaseResponseDTO TicketPurchaseResponse { get; set; } = new();
    }
}
