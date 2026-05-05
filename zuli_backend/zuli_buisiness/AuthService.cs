using Microsoft.AspNetCore.Identity;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class AuthService : IAuthService
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        public async Task<AuthResultDTO> LoginAsync(LoginRequestDTO request)
        {
            var failedLogin = new AuthResultDTO
            {
                Success = false,
                Message = "Correo o contraseña incorrectos.",
                User = null,
                Response = new LoginResponseDTO
                {
                    Success = false,
                    Message = "Correo o contraseña incorrectos."
                }
            };

            string businessEmail = request.BusinessEmail.Trim().ToLower();

            AppUser? user = await _userRepository.GetByBusinessEmailAsync(businessEmail);

            if (user == null)
            {
                return failedLogin;
            }

            if (!user.IsActive)
            {
                return failedLogin;
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return failedLogin;
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                return failedLogin;
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                }

                await _userRepository.UpdateLoginStateAsync(user);

                return failedLogin;
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await _userRepository.UpdateLoginStateAsync(user);

            return new AuthResultDTO
            {
                Success = true,
                Message = "Login exitoso.",
                User = user,
                Response = new LoginResponseDTO
                {
                    Success = true,
                    Message = "Login exitoso.",
                    BusinessEmail = user.BusinessEmail,
                    BusinessId = user.BusinessId,
                    UserRole = user.UserRole
                }
            };
        }

        public LoginResponseDTO BuildAuthenticatedUserResponse(string? businessEmail, string? businessId, string? userRole)
        {
            return new LoginResponseDTO
            {
                Success = true,
                Message = "Usuario autenticado.",
                BusinessEmail = businessEmail,
                BusinessId = businessId,
                UserRole = userRole
            };
        }

        public LoginResponseDTO BuildLogoutResponse()
        {
            return new LoginResponseDTO
            {
                Success = true,
                Message = "Sesión cerrada."
            };
        }
    }
}