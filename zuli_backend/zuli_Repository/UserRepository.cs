using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _dapperContext;

        public UserRepository(DapperContext dapperContext)
        {
            this._dapperContext = dapperContext;
        }

        public async Task<AppUser?> GetByBusinessEmailAsync(string businessEmail)
        {
            using var connection = dapperContext.CreateConnection();

            var sql = @"
                SELECT
                    UserId,
                    BusinessEmail,
                    BusinessId,
                    UserRole,
                    PasswordHash,
                    IsActive,
                    FailedLoginAttempts,
                    LockoutEnd,
                    ManagedByAdminId
                FROM AirlineUser
                WHERE BusinessEmail = @BusinessEmail;
            ";

            return await connection.QueryFirstOrDefaultAsync<AppUser>(
                sql,
                new { BusinessEmail = businessEmail }
            );
        }

        public async Task UpdateLoginStateAsync(AppUser user)
        {
            using var connection = dapperContext.CreateConnection();

            var sql = @"
                UPDATE AirlineUser
                SET
                    FailedLoginAttempts = @FailedLoginAttempts,
                    LockoutEnd = @LockoutEnd
                WHERE UserId = @UserId;
            ";

            await connection.ExecuteAsync(sql, new
            {
                user.UserId,
                user.FailedLoginAttempts,
                user.LockoutEnd
            });
        }
    }
}