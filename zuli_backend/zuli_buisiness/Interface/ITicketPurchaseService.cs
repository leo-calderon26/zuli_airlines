using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface ITicketPurchaseService
    {
        Task<TicketPurchaseResponseDTO> Purchase(TicketPurchaseRequestDTO request);
    }
}
