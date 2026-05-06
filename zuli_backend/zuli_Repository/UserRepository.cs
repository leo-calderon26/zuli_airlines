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
            using var connection = _dapperContext.CreateConnection();

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
            using var connection = _dapperContext.CreateConnection();

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
        public async Task<bool> IsAdmin(string businesId)
        {
            using var connection = _dapperContext.CreateConnection();
            var sql = @"
            SELECT CASE WHEN EXISTS (
            SELECT 1
            FROM AirlineUser
            WHERE BusinessId = @BusinessId AND UserRole = 'Administrator'
            ) THEN 1 ELSE 0 END";
            return await connection.ExecuteScalarAsync<bool>(sql, new { BusinessId = businesId });
        }
        public async Task<Guid> GetUserId(string businesId)
        {
            using var connection = _dapperContext.CreateConnection();
            var sql = @"
            SELECT UserId
            FROM AirlineUser
            WHERE BusinessId = @BusinessId";
            var userId = await connection.ExecuteScalarAsync<Guid?>(sql, new { BusinessId = businesId });
            return userId ?? Guid.Empty;
        }

        public async Task<string> GetBusinessId(Guid userId)
        {
            using var connection = _dapperContext.CreateConnection();
            var sql = @"
            SELECT BusinessId
            FROM AirlineUser
            WHERE UserId = @UserId";
            var businessId = await connection.ExecuteScalarAsync<string?>(sql, new { UserId = userId });
            return businessId ?? string.Empty;
        }
    }
}