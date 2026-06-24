using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class UserUpdateMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterUserRequestDTO, AppUser>()
                .Ignore(dest => dest.UserId)
                .Ignore(dest => dest.PersonId)
<<<<<<< HEAD
=======
                .Ignore(dest => dest.NationalId)
>>>>>>> database/develop
                .Ignore(dest => dest.BusinessId)
                .Ignore(dest => dest.PasswordHash)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.FailedLoginAttempts)
                .Ignore(dest => dest.LockoutEnd)
                .Ignore(dest => dest.ManagedByAdminId)
                .Ignore(dest => dest.ActivationTokenHash)
<<<<<<< HEAD
                .Map(dest => dest.NationalId, src => src.NationalId.Trim())
=======
>>>>>>> database/develop
                .Map(dest => dest.FirstName, src => src.FirstName.Trim())
                .Map(dest => dest.FirstLastName, src => src.FirstLastName.Trim())
                .Map(dest => dest.SecondLastName, src => src.SecondLastName.Trim())
                .Map(dest => dest.BusinessEmail, src => src.BusinessEmail.Trim().ToLower())
                .Map(dest => dest.Email, src => src.BusinessEmail.Trim().ToLower())
                .Map(dest => dest.UserRole, src => src.UserRole.Trim());
        }
    }
}