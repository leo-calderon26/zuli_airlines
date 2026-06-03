using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPassengerCreationService
    {
        Task<List<int>> CreateAllPassengers(List<PassengerTicketDTO> passengers);
    }
}
