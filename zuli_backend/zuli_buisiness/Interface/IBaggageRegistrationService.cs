using zuli_Business.DTO;
using zuli_Data;

namespace zuli_Business.Interface
{
    public interface IBaggageRegistrationService
    {
        Task RegisterAllBaggage(
            List<PassengerTicketDTO> passengers,
            List<int> passengerIds,
            int reservationId,
            IUnitOfWork? uow = null);

        Task ValidateBaggageCapacity(
            List<PassengerTicketDTO> passengers,
            List<Guid> flightIds
        );
    }
}
