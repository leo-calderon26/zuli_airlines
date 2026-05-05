using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly RegisterUserValidator _registerUserValidator;
        private readonly ActivateAccountValidator _activateAccountValidator;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public UserRegistrationService(
            IUserRepository userRepository,
            IEmailService emailService,
            RegisterUserValidator registerUserValidator,
            ActivateAccountValidator activateAccountValidator,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _registerUserValidator = registerUserValidator;
            _activateAccountValidator = activateAccountValidator;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        public async Task<RegisterUserResponseDTO> RegisterUserAsync(
            RegisterUserRequestDTO request,
            Guid adminUserId)
        {
            _registerUserValidator.Validate(request);

            string nationalId = request.NationalId.Trim();
            string businessEmail = request.BusinessEmail.Trim().ToLower();
            string firstName = request.FirstName.Trim();
            string firstLastName = request.FirstLastName.Trim();
            string secondLastName = request.SecondLastName.Trim();
            string userRole = request.UserRole.Trim();

            await ValidateUniqueUserAsync(nationalId, businessEmail);

            string activationToken = GenerateSecureToken();
            string activationTokenHash = HashToken(activationToken);

            var user = new AppUser
            {
                UserId = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                NationalId = nationalId,
                FirstName = firstName,
                FirstLastName = firstLastName,
                SecondLastName = secondLastName,
                Email = null,

                BusinessEmail = businessEmail,
                BusinessId = nationalId,
                UserRole = userRole,
                PasswordHash = null,

                IsActive = false,
                FailedLoginAttempts = 0,
                LockoutEnd = null,

                ManagedByAdminId = adminUserId,
                ActivationTokenHash = activationTokenHash
            };

            await _userRepository.CreatePendingUserAsync(user);

            string fullName = $"{user.FirstName} {user.FirstLastName} {user.SecondLastName}";
            string activationLink = BuildActivationLink(activationToken);

            await _emailService.SendActivationEmailAsync(
                user.BusinessEmail,
                fullName,
                activationLink
            );

            return new RegisterUserResponseDTO
            {
                Success = true,
                Message = "Usuario registrado. Se envió el correo de activación.",
                UserId = user.UserId,
                PersonId = user.PersonId,
                BusinessEmail = user.BusinessEmail
            };
        }

        public async Task<BasicResponseDTO> ActivateAccountAsync(ActivateAccountRequestDTO request)
        {
            _activateAccountValidator.Validate(request);

            string tokenHash = HashToken(request.Token);

            AppUser? user = await _userRepository.GetByActivationTokenHashAsync(tokenHash);

            if (user == null)
            {
                ThrowValidationError("token", "El enlace de activación no es válido.");
            }

            if (user!.IsActive)
            {
                ThrowValidationError("token", "La cuenta ya fue activada.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            user.IsActive = true;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.ActivationTokenHash = null;

            await _userRepository.ActivateUserAsync(user);

            return new BasicResponseDTO
            {
                Success = true,
                Message = "Cuenta activada correctamente."
            };
        }

        private async Task ValidateUniqueUserAsync(string nationalId, string businessEmail)
        {
            AppUser? userByNationalId = await _userRepository.GetByNationalIdAsync(nationalId);

            if (userByNationalId != null)
            {
                ThrowValidationError("nationalId", "Ya existe una persona registrada con esa cédula.");
            }

            AppUser? userByEmail = await _userRepository.GetByBusinessEmailAsync(businessEmail);

            if (userByEmail != null)
            {
                ThrowValidationError("businessEmail", "Ya existe un usuario con ese correo institucional.");
            }
        }

        private string BuildActivationLink(string token)
        {
            string frontendBaseUrl = _configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException("Frontend:BaseUrl is not configured.");
        }

        private static string GenerateSecureToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private static string HashToken(string token)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            byte[] hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }

        private static void ThrowValidationError(string fieldName, string message)
        {
            var errors = new Dictionary<string, List<string>>
            {
                [fieldName] = new List<string>
                {
                    message
                }
            };

            throw new ZuliValidationException(errors);
        }
    }
}