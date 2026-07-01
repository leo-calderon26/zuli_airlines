using zuli_Business.DTO;
using zuli_Business.DTO.ReservationSearch;

namespace zuli_Business.Interface
{
    public interface IBaggageRegistrationService
    {
        Task RegisterAllBaggage(
            List<PassengerTicketDTO> passengers,
            List<int> passengerIds,
            int reservationId);

        Task ValidateBaggageCapacity(
            List<PassengerTicketDTO> passengers,
            List<Guid> flightIds
        );
        Task AddAdditionalBaggageTransactional(
            string reservationCode,
            List<AdditionalBaggagePassengerDTO> passengers
        );
    }
}
