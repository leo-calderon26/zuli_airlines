using zuli_buisiness.DTO;

namespace zuli_buisiness.Interface
{
	public interface IAuthService
	{
		Task<AuthResultDTO> LoginAsync(LoginRequestDTO request);
	}
}