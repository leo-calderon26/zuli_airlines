using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByBusinessEmailAsync(string businessEmail);
        Task<AppUser?> GetByNationalIdAsync(string nationalId);
        Task<AppUser?> GetByActivationTokenHashAsync(string activationTokenHash);

        Task CreatePendingUserAsync(AppUser user);
        Task UpdateLoginStateAsync(AppUser user);
        Task ActivateUserAsync(AppUser user);

        Task<bool> IsAdmin(string businesId);
        Task<Guid> GetUserId(string businesId);
    }
}