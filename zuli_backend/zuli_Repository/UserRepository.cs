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
                    au.UserId,
                    au.PersonId,
                    p.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    p.Email,
                    au.BusinessEmail,
                    au.BusinessId,
                    au.UserRole,
                    au.PasswordHash,
                    au.IsActive,
                    au.FailedLoginAttempts,
                    au.LockoutEnd,
                    au.ManagedByAdminId,
                    au.ActivationTokenHash
                FROM AirlineUser au
                INNER JOIN Person p ON au.PersonId = p.PersonId
                WHERE au.BusinessEmail = @BusinessEmail;
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
                    au.UserId,
                    au.PersonId,
                    p.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    p.Email,
                    au.BusinessEmail,
                    au.BusinessId,
                    au.UserRole,
                    au.PasswordHash,
                    au.IsActive,
                    au.FailedLoginAttempts,
                    au.LockoutEnd,
                    au.ManagedByAdminId,
                    au.ActivationTokenHash
                FROM AirlineUser au
                INNER JOIN Person p ON au.PersonId = p.PersonId
                WHERE p.NationalId = @NationalId;
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
                    au.UserId,
                    au.PersonId,
                    p.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    p.Email,
                    au.BusinessEmail,
                    au.BusinessId,
                    au.UserRole,
                    au.PasswordHash,
                    au.IsActive,
                    au.FailedLoginAttempts,
                    au.LockoutEnd,
                    au.ManagedByAdminId,
                    au.ActivationTokenHash
                FROM AirlineUser au
                INNER JOIN Person p ON au.PersonId = p.PersonId
                WHERE au.ActivationTokenHash = @ActivationTokenHash;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { ActivationTokenHash = activationTokenHash }
            );
        }

        public async Task CreatePendingUserAsync(AppUser user)
        {
            using var connection = _dapperContext.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var personSql = @"
                    INSERT INTO Person (
                        NationalId,
                        FirstName,
                        FirstLastName,
                        SecondLastName,
                        Email
                    )
                    VALUES (
                        @NationalId,
                        @FirstName,
                        @FirstLastName,
                        @SecondLastName,
                        @Email
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

                int personId = await connection.QuerySingleAsync<int>(
                    personSql,
                    user,
                    transaction
                );

                user.PersonId = personId;

                var userSql = @"
                    INSERT INTO AirlineUser (
                        UserId,
                        PersonId,
                        BusinessEmail,
                        BusinessId,
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
                        @PersonId,
                        @BusinessEmail,
                        @BusinessId,
                        @UserRole,
                        @PasswordHash,
                        @IsActive,
                        @FailedLoginAttempts,
                        @LockoutEnd,
                        @ManagedByAdminId,
                        @ActivationTokenHash
                    );
                ";

                await connection.ExecuteAsync(userSql, user, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
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