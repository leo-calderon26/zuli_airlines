using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IReservationCreationService
    {
        Task<int> CreateReservation(string code, TicketPurchaseRequestDTO request, decimal total, int buyerId);
    }
}
