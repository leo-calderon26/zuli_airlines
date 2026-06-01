using System;
using System.Threading.Tasks;
using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface ITicketService
    {
        Task<TicketPurchaseResponseDTO> Purchase(TicketPurchaseRequestDTO request);
    }
}
