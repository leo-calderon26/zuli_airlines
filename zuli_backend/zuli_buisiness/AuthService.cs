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

        private readonly IUserRepository userRepository;
        private readonly PasswordHasher<AppUser> passwordHasher;

        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            passwordHasher = new PasswordHasher<AppUser>();
        }

        public async Task<AuthResultDTO> LoginAsync(LoginRequestDTO request)
        {
            string businessEmail = request.BusinessEmail.Trim().ToLower();

            AppUser? user = await userRepository.GetByBusinessEmailAsync(businessEmail);

            if (user == null)
            {
                return FailedLogin();
            }

            if (!user.IsActive)
            {
                return FailedLogin();
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                return FailedLogin();
            }

            PasswordVerificationResult passwordResult = passwordHasher.VerifyHashedPassword(
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

                await userRepository.UpdateLoginStateAsync(user);

                return FailedLogin();
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await userRepository.UpdateLoginStateAsync(user);

            return new AuthResultDTO
            {
                Success = true,
                Message = "Login exitoso.",
                User = user
            };
        }

        private AuthResultDTO FailedLogin()
        {
            return new AuthResultDTO
            {
                Success = false,
                Message = "Correo o contraseña incorrectos."
            };
        }
    }
}