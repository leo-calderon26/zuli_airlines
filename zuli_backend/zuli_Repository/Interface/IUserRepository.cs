using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByBusinessEmailAsync(string businessEmail);
        Task UpdateLoginStateAsync(AppUser user);
        Task<bool> IsAdmin(string businesId);
        Task<Guid> GetUserId(string businesId);
        Task<string> GetBusinessId(Guid userId);
    }
}