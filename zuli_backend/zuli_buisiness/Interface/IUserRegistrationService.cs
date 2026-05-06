using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IUserRegistrationService
    {
        Task<RegisterUserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request, Guid adminUserId);
        Task<BasicResponseDTO> ActivateAccountAsync(ActivateAccountRequestDTO request);
    }
}