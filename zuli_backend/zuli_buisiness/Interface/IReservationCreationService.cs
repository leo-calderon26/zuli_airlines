using zuli_Business.DTO;
using zuli_Data;

namespace zuli_Business.Interface
{
    public interface IReservationCreationService
    {
        Task<(int ReservationId, string ReservationCode)> CreateReservation(
            TicketPurchaseRequestDTO request,
            decimal total,
            int buyerId,
            IUnitOfWork? uow = null);
    }
}
