using System.Transactions;
using System.Transactions;
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

            // (logging removed)

            var sql = @"
                SELECT
                    au.UserId,
                    au.PersonId,
                    au.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    pe.Email,
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
                LEFT JOIN PersonEmail pe ON p.PersonId = pe.PersonId
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

            // (logging removed)

            var sql = @"
                SELECT
                    au.UserId,
                    au.PersonId,
                    au.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    pe.Email,
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
                LEFT JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                WHERE au.NationalId = @NationalId;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { NationalId = nationalId }
            );
        }

        public async Task<AppUser?> GetByUserIdAsync(Guid userId)
        {
            using var connection = _dapperContext.CreateConnection();

            // (logging removed)

            var sql = @"
                SELECT
                    au.UserId,
                    au.PersonId,
                    au.BusinessId AS NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    pe.Email,
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
                LEFT JOIN Person p ON au.PersonId = p.PersonId
                LEFT JOIN PersonEmail pe ON au.PersonId = pe.PersonId
                WHERE au.UserId = @UserId;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { UserId = userId }
            );
        }

        public async Task<AppUser?> GetByActivationTokenHashAsync(string activationTokenHash)
        {
            using var connection = _dapperContext.CreateConnection();

            // (logging removed)

            var sql = @"
                SELECT
                    au.UserId,
                    au.PersonId,
                    au.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    pe.Email,
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
                LEFT JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                WHERE au.ActivationTokenHash = @ActivationTokenHash;
            ";

            return await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new { ActivationTokenHash = activationTokenHash }
            );
        }

        public async Task CreatePendingUserAsync(AppUser user)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var connection = _dapperContext.CreateConnection();

            var personSql = @"
                INSERT INTO Person (
                    FirstName,
                    FirstLastName,
                    SecondLastName,
                    BirthDate,
                    Gender
                )
                VALUES (
                    @FirstName,
                    @FirstLastName,
                    @SecondLastName,
                    '',
                    ''
                );
            var personSql = @"
                INSERT INTO Person (
                    FirstName,
                    FirstLastName,
                    SecondLastName,
                    BirthDate,
                    Gender
                )
                VALUES (
                    @FirstName,
                    @FirstLastName,
                    @SecondLastName,
                    '',
                    ''
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            int personId = await connection.QuerySingleAsync<int>(
                personSql,
                user
            );

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var emailSql = @"
                    INSERT INTO PersonEmail (PersonId, Email)
                    VALUES (@PersonId, @Email);";

                await connection.ExecuteAsync(emailSql, new { PersonId = personId, user.Email });
            }
            int personId = await connection.QuerySingleAsync<int>(
                personSql,
                user
            );

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var emailSql = @"
                    INSERT INTO PersonEmail (PersonId, Email)
                    VALUES (@PersonId, @Email);";

                await connection.ExecuteAsync(emailSql, new { PersonId = personId, user.Email });
            }

            user.PersonId = personId;
            user.PersonId = personId;

            var userSql = @"
                INSERT INTO AirlineUser (
                    UserId,
                    PersonId,
                    NationalId,
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
                    @NationalId,
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
            var userSql = @"
                INSERT INTO AirlineUser (
                    UserId,
                    PersonId,
                    NationalId,
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
                    @NationalId,
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

            await connection.ExecuteAsync(userSql, user);

            scope.Complete();
            await connection.ExecuteAsync(userSql, user);

            scope.Complete();
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

        public async Task UpdateUserAsync(AppUser user)
        {
            using var connection = _dapperContext.CreateConnection();

            await connection.ExecuteAsync(
                "dbo.sp_updateUser",
                new
                {
                    user.UserId,
                    user.PersonId,
                    user.NationalId,
                    user.FirstName,
                    user.FirstLastName,
                    user.SecondLastName,
                    user.BusinessEmail,
                    user.UserRole
                },
                commandType: System.Data.CommandType.StoredProcedure
            );
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
        public async Task<(List<AppUser> Users, int TotalItems)> GetUsersAsync(
            string searchType,
            string search,
            int page,
            int pageSize)


        {
            using var connection = _dapperContext.CreateConnection();

            int offset = (page - 1) * pageSize;
            string normalizedSearch = $"%{search}%";

            string whereClause = searchType switch
            {
                "email" => "(au.BusinessEmail LIKE @Search OR pe.Email LIKE @Search)",
                "email" => "(au.BusinessEmail LIKE @Search OR pe.Email LIKE @Search)",

                "nationalId" => "au.NationalId LIKE @Search",

                "name" => @"
                    (
                        p.FirstName LIKE @Search
                        OR p.FirstLastName LIKE @Search
                        OR p.SecondLastName LIKE @Search
                        OR CONCAT(p.FirstName, ' ', p.FirstLastName) LIKE @Search
                        OR CONCAT(p.FirstName, ' ', p.FirstLastName, ' ', p.SecondLastName) LIKE @Search
                    )",

                _ => @"
                    (
                        au.BusinessEmail LIKE @Search
                        OR au.NationalId LIKE @Search
                        OR p.FirstName LIKE @Search
                        OR p.FirstLastName LIKE @Search
                        OR p.SecondLastName LIKE @Search
                        OR CONCAT(p.FirstName, ' ', p.FirstLastName) LIKE @Search
                        OR CONCAT(p.FirstName, ' ', p.FirstLastName, ' ', p.SecondLastName) LIKE @Search
                        OR pe.Email LIKE @Search
                        OR pe.Email LIKE @Search
                    )"
            };

            var countSql = $@"
                SELECT COUNT(*)
                FROM AirlineUser au
                INNER JOIN Person p ON au.PersonId = p.PersonId
                LEFT JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                WHERE {whereClause};
            ";

            var usersSql = $@"
                SELECT
                    au.UserId,
                    au.PersonId,
                    au.NationalId,
                    p.FirstName,
                    p.FirstLastName,
                    p.SecondLastName,
                    pe.Email,
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
                LEFT JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                WHERE {whereClause}
                ORDER BY p.FirstName, p.FirstLastName, p.SecondLastName
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            ";

            var parameters = new
            {
                Search = normalizedSearch,
                Offset = offset,
                PageSize = pageSize
            };

            int totalItems = await connection.ExecuteScalarAsync<int>(
                countSql,
                parameters
            );

            List<AppUser> users = (
                await connection.QueryAsync<AppUser>(
                    usersSql,
                    parameters
                )
            ).ToList();

            return (users, totalItems);
        }
    }
}