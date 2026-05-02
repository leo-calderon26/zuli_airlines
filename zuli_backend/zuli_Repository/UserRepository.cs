using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using zuli_Data.Entities;

namespace zuli_Repository
{
    public class UserRepository
    {
        private readonly string connectionString;

        public UserRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<AppUser?> GetByBusinessEmailAsync(string businessEmail)
        {
            using SqlConnection connection = new SqlConnection(connectionString);

            string sql = @"
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
                FROM [User]
                WHERE BusinessEmail = @BusinessEmail;
            ";

            return await connection.QueryFirstOrDefaultAsync<AppUser>(
                sql,
                new { BusinessEmail = businessEmail }
            );
        }

        public async Task UpdateLoginStateAsync(AppUser user)
        {
            using SqlConnection connection = new SqlConnection(connectionString);

            string sql = @"
                UPDATE [User]
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