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
            _dapperContext = dapperContext;
        }

        public async Task<AppUser?> GetByBusinessEmailAsync(string businessEmail)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                SELECT
                    UserId,
                    NationalId,
                    BusinessEmail,
                    BusinessId,
                    FirstName,
                    FirstLastName,
                    SecondLastName,
                    UserRole,
                    PasswordHash,
                    IsActive,
                    FailedLoginAttempts,
                    LockoutEnd,
                    ManagedByAdminId,
                    ActivationTokenHash
                FROM AirlineUser
                WHERE BusinessEmail = @BusinessEmail;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { BusinessEmail = businessEmail }
            );
        }

        public async Task<AppUser?> GetByNationalIdAsync(string nationalId)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                SELECT
                    UserId,
                    NationalId,
                    BusinessEmail,
                    BusinessId,
                    FirstName,
                    FirstLastName,
                    SecondLastName,
                    UserRole,
                    PasswordHash,
                    IsActive,
                    FailedLoginAttempts,
                    LockoutEnd,
                    ManagedByAdminId,
                    ActivationTokenHash
                FROM AirlineUser
                WHERE NationalId = @NationalId;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { NationalId = nationalId }
            );
        }

        public async Task<AppUser?> GetByActivationTokenHashAsync(string activationTokenHash)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                SELECT
                    UserId,
                    NationalId,
                    BusinessEmail,
                    BusinessId,
                    FirstName,
                    FirstLastName,
                    SecondLastName,
                    UserRole,
                    PasswordHash,
                    IsActive,
                    FailedLoginAttempts,
                    LockoutEnd,
                    ManagedByAdminId,
                    ActivationTokenHash
                FROM AirlineUser
                WHERE ActivationTokenHash = @ActivationTokenHash;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { ActivationTokenHash = activationTokenHash }
            );
        }

        public async Task CreatePendingUserAsync(AppUser user)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                INSERT INTO AirlineUser (
                    UserId,
                    NationalId,
                    BusinessEmail,
                    BusinessId,
                    FirstName,
                    FirstLastName,
                    SecondLastName,
                    UserRole,
                    PasswordHash,
                    IsActive,
                    FailedLoginAttempts,
                    LockoutEnd,
                    ManagedByAdminId,
                    ActivationTokenHash
                )
                VALUES (
                    @UserId,
                    @NationalId,
                    @BusinessEmail,
                    @BusinessId,
                    @FirstName,
                    @FirstLastName,
                    @SecondLastName,
                    @UserRole,
                    @PasswordHash,
                    @IsActive,
                    @FailedLoginAttempts,
                    @LockoutEnd,
                    @ManagedByAdminId,
                    @ActivationTokenHash
                );
            ";

            await connection.ExecuteAsync(sql, user);
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

            await connection.ExecuteAsync(sql, user);
        }

        public async Task ActivateUserAsync(AppUser user)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                UPDATE AirlineUser
                SET
                    PasswordHash = @PasswordHash,
                    IsActive = 1,
                    FailedLoginAttempts = 0,
                    LockoutEnd = NULL,
                    ActivationTokenHash = NULL
                WHERE UserId = @UserId;
            ";

            await connection.ExecuteAsync(sql, user);
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
    }
}