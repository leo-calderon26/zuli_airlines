using zuli_Buisiness.DTO;

namespace zuli_Buisiness.Interface
{
	public interface IAuthService
	{
		Task<AuthResultDTO> LoginAsync(LoginRequestDTO request);
	}
}