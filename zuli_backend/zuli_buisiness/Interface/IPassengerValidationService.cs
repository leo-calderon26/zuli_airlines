using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPassengerValidationService
    {
        Task ValidateRequestPassengersAreUnique(List<PassengerTicketDTO> passengers);
        Task ValidatePassengersDoNotExistInFlights(
            List<PassengerTicketDTO> passengers,
            List<Guid> flightIds
        );
    }
}
