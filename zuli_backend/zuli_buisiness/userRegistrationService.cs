using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Mapster;
using FluentValidation;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;
using Microsoft.AspNetCore.Http;

namespace zuli_Business
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly FluentValidation.IValidator<RegisterUserRequestDTO> _registerUserValidator;
        private readonly ActivateAccountValidator _activateAccountValidator;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public UserRegistrationService(
            IUserRepository userRepository,
            IEmailService emailService,
            FluentValidation.IValidator<RegisterUserRequestDTO> registerUserValidator,
            ActivateAccountValidator activateAccountValidator,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _registerUserValidator = registerUserValidator;
            _activateAccountValidator = activateAccountValidator;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<AppUser>();
            // logger removed
        }

        public async Task<RegisterUserResponseDTO> RegisterUserAsync(
            RegisterUserRequestDTO request,
            Guid adminUserId)
        {
            var validationResult = await _registerUserValidator.ValidateAsync(request);
            validationResult.ThrowIfInvalid();

            string nationalId = request.NationalId.Trim();
            string businessEmail = request.BusinessEmail.Trim().ToLower();
            string firstName = request.FirstName.Trim();
            string firstLastName = request.FirstLastName.Trim();
            string secondLastName = request.SecondLastName.Trim();
            string userRole = request.UserRole.Trim();

            // logging removed

            await ValidateUniqueUserAsync(nationalId, businessEmail);

            string activationToken = GenerateSecureToken();
            string activationTokenHash = HashToken(activationToken);

            var user = request.Adapt<AppUser>();
            user.UserId = Guid.NewGuid();
            user.ManagedByAdminId = adminUserId;
            user.ActivationTokenHash = activationTokenHash;

            if (string.IsNullOrWhiteSpace(user.BusinessId))
            {
                int num = RandomNumberGenerator.GetInt32(10000000, 100000000);
                user.BusinessId = num.ToString();
            }

            try
            {
                await _userRepository.CreatePendingUserAsync(user);
            }
            catch (Exception)
            {
                throw;
            }

            string fullName = $"{user.FirstName} {user.FirstLastName} {user.SecondLastName}";
            string activationLink = BuildActivationLink(activationToken);

            await _emailService.SendActivationEmailAsync(
                user.BusinessEmail,
                fullName,
                activationLink
            );

            return new RegisterUserResponseDTO
            {
                StatusCode = StatusCodes.Status200OK,
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
                StatusCode = StatusCodes.Status200OK,
                Message = "Cuenta activada correctamente."
            };
        }

        public async Task<BasicResponseDTO> UpdateUserAsync(Guid userId, RegisterUserRequestDTO request)
        {
            var validationResult = await _registerUserValidator.ValidateAsync(request);
            validationResult.ThrowIfInvalid();

            AppUser? existingUser = await _userRepository.GetByUserIdAsync(userId);

            if (existingUser == null)
            {
                throw new ZuliNotFoundException($"No existe un usuario con id {userId}.");
            }

            string businessEmail = request.BusinessEmail.Trim().ToLower();
            AppUser? userByEmail = await _userRepository.GetByBusinessEmailAsync(businessEmail);

            if (userByEmail != null && userByEmail.UserId != userId)
            {
                ThrowValidationError("businessEmail", "Ya existe un usuario con ese correo institucional.");
            }

            string nationalId = request.NationalId.Trim();
            AppUser? userByNationalId = await _userRepository.GetByNationalIdAsync(nationalId);

            if (userByNationalId != null && userByNationalId.UserId != userId)
            {
                ThrowValidationError("nationalId", "Ya existe una persona registrada con esa cédula.");
            }

            request.Adapt(existingUser, TypeAdapterConfig.GlobalSettings);

            await _userRepository.UpdateUserAsync(existingUser);

            return new BasicResponseDTO
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Usuario actualizado correctamente."
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

            return $"{frontendBaseUrl}/users/activation?token={Uri.EscapeDataString(token)}";
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
        public async Task<UserSearchResponseDTO> GetUsersAsync(
            string? searchType,
            string? search,
            int page,
            int pageSize)
        {
            string normalizedSearchType = NormalizeSearchType(searchType);
            string normalizedSearch = search?.Trim() ?? string.Empty;

            int normalizedPage = page <= 0 ? 1 : page;
            int normalizedPageSize = pageSize <= 0 ? 10 : pageSize;

            if (normalizedPageSize > 50)
            {
                normalizedPageSize = 50;
            }

            var result = await _userRepository.GetUsersAsync(
                normalizedSearchType,
                normalizedSearch,
                normalizedPage,
                normalizedPageSize
            );

            int totalPages = result.TotalItems == 0
                ? 1
                : (int)Math.Ceiling(result.TotalItems / (double)normalizedPageSize);

            return new UserSearchResponseDTO
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Usuarios obtenidos correctamente.",
                Page = normalizedPage,
                PageSize = normalizedPageSize,
                TotalItems = result.TotalItems,
                TotalPages = totalPages,
                Users = result.Users.Select(user => user.Adapt<UserListItemDTO>()).ToList()
            };
        }
        private static string NormalizeSearchType(string? searchType)
        {
            string normalizedSearchType = searchType?.Trim() ?? "all";

            return normalizedSearchType switch
            {
                "email" => "email",
                "name" => "name",
                "nationalId" => "nationalId",
                _ => "all"
            };
        }

    }
}