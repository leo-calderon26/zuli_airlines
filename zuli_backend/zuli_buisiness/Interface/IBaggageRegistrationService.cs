using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IBaggageRegistrationService
    {
        Task RegisterAllBaggage(List<PassengerTicketDTO> passengers, List<int> passengerIds, int reservationId);
    }
}
