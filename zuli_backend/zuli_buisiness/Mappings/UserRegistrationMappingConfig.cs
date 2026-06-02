using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class UserRegistrationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterUserRequestDTO, AppUser>()
                .Map(dest => dest.NationalId, src => src.NationalId.Trim())
                .Map(dest => dest.FirstName, src => src.FirstName.Trim())
                .Map(dest => dest.FirstLastName, src => src.FirstLastName.Trim())
                .Map(dest => dest.SecondLastName, src => src.SecondLastName.Trim())
                .Map(dest => dest.Email, _ => (string?)null)
                .Map(dest => dest.BusinessEmail, src => src.BusinessEmail.Trim().ToLower())
                .Map(dest => dest.BusinessId, src => src.NationalId.Trim())
                .Map(dest => dest.UserRole, src => src.UserRole.Trim())
                .Map(dest => dest.PasswordHash, _ => (string?)null)
                .Map(dest => dest.IsActive, _ => false)
                .Map(dest => dest.FailedLoginAttempts, _ => 0)
                .Map(dest => dest.LockoutEnd, _ => (DateTime?)null)
                .Map(dest => dest.ManagedByAdminId, _ => (Guid?)null)
                .Map(dest => dest.ActivationTokenHash, _ => (string?)null);

            config.NewConfig<AppUser, UserListItemDTO>();
        }
    }
}