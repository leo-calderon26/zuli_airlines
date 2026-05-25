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
using Microsoft.AspNetCore.Http;

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
            ValidateUpdateRequest(request);

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

            existingUser.FirstName = request.FirstName.Trim();
            existingUser.FirstLastName = request.FirstLastName.Trim();
            existingUser.SecondLastName = request.SecondLastName.Trim();
            existingUser.BusinessEmail = businessEmail;
            existingUser.Email = businessEmail;
            existingUser.UserRole = request.UserRole.Trim();

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
                Users = result.Users.Select(user => new UserListItemDTO
                {
                    UserId = user.UserId,
                    PersonId = user.PersonId,
                    NationalId = user.NationalId,
                    FirstName = user.FirstName,
                    FirstLastName = user.FirstLastName,
                    SecondLastName = user.SecondLastName,
                    BusinessEmail = user.BusinessEmail,
                    UserRole = user.UserRole,
                    IsActive = user.IsActive
                }).ToList()
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

        private static void ValidateUpdateRequest(RegisterUserRequestDTO? request)
        {
            var errors = new Dictionary<string, List<string>>();

            if (request == null)
            {
                errors["request"] = new List<string>
                {
                    "Solicitud inválida."
                };

                throw new ZuliValidationException(errors);
            }

            if (string.IsNullOrWhiteSpace(request.NationalId))
            {
                errors["nationalId"] = new List<string> { "La cédula es obligatoria." };
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(request.NationalId.Trim(), @"^\d{9}$"))
            {
                errors["nationalId"] = new List<string> { "La cédula debe tener exactamente 9 dígitos, sin espacios ni guiones." };
            }

            if (string.IsNullOrWhiteSpace(request.BusinessEmail))
            {
                errors["businessEmail"] = new List<string> { "El correo institucional es obligatorio." };
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(request.BusinessEmail.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors["businessEmail"] = new List<string> { "El correo institucional no tiene un formato válido." };
            }

            ValidateTextField("firstName", request.FirstName, "El primer nombre", errors);
            ValidateTextField("firstLastName", request.FirstLastName, "El primer apellido", errors);
            ValidateTextField("secondLastName", request.SecondLastName, "El segundo apellido", errors);
            ValidateUserRole(request.UserRole, errors);

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }

        private static void ValidateTextField(string fieldName, string value, string displayName, Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors[fieldName] = new List<string> { $"{displayName} es obligatorio." };
                return;
            }

            if (value.Trim().Length > 50)
            {
                errors[fieldName] = new List<string> { $"{displayName} no puede superar los 50 caracteres." };
            }
        }

        private static void ValidateUserRole(string userRole, Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(userRole))
            {
                errors["userRole"] = new List<string> { "El tipo de usuario es obligatorio." };
                return;
            }

            string trimmedRole = userRole.Trim();

            if (trimmedRole != "Administrator" && trimmedRole != "Operator")
            {
                errors["userRole"] = new List<string> { "El tipo de usuario no es válido." };
            }
        }

    }
}