using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByBusinessEmailAsync(string businessEmail);
        Task<AppUser?> GetByNationalIdAsync(string nationalId);
        Task<AppUser?> GetByUserIdAsync(Guid userId);
        Task<AppUser?> GetByActivationTokenHashAsync(string activationTokenHash);

        Task CreatePendingUserAsync(AppUser user);
        Task UpdateLoginStateAsync(AppUser user);
        Task ActivateUserAsync(AppUser user);
        
        Task UpdateUserAsync(AppUser user);

        Task<string> DeleteUserAsync(Guid userId);

        Task<bool> IsAdmin(string businesId);
        Task<Guid> GetUserId(string businesId);
        Task<string> GetBusinessId(Guid userId);

        Task<(List<AppUser> Users, int TotalItems)> GetUsersAsync(
            string searchType,
            string search,
            int page,
            int pageSize
        );
    }
}