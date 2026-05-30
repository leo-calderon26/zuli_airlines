using zuli_Business.DTO;

namespace zuli_Business.Interface
{
	public interface IAuthService
	{
		Task<AuthResultDTO> LoginAsync(LoginRequestDTO request);
		LoginResponseDTO BuildAuthenticatedUserResponse(string? businessEmail, string? businessId, string? userRole);
        LoginResponseDTO BuildLogoutResponse();
	}
}