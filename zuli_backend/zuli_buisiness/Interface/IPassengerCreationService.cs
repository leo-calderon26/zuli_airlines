using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPassengerCreationService
    {
        Task<(List<int> passengerIds, int buyerId)> CreateAllPassengers(
            List<PassengerTicketDTO> passengers,
            BuyerTicketDTO buyer,
            zuli_Data.IUnitOfWork? uow = null);
    }
}
