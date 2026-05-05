using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByBusinessEmailAsync(string businessEmail);
        Task UpdateLoginStateAsync(AppUser user);
    }
}