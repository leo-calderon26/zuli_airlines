using zuli_Business.DTO;

namespace zuli_Business.Interface
{
	public interface IAuthService
	{
		Task<AuthResultDTO> LoginAsync(LoginRequestDTO request);
	}
}