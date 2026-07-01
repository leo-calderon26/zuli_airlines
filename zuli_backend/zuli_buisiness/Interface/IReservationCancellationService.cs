using zuli_Business.DTO;
using zuli_Business.DTO.Cancellation;

namespace zuli_Business.Interface
{
    public interface IReservationCancellationService
    {
        Task<BasicResponseDTO> RequestCancellationAsync(RequestCancellationRequestDTO request);
        Task<BasicResponseDTO> ConfirmCancellationAsync(ConfirmCancellationRequestDTO request);
    }
}
