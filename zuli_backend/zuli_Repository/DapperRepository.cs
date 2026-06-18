using System.Data;
using zuli_Data;

namespace zuli_Repository
{
    public abstract class DapperRepository
    {
        private readonly DapperContext _context;

        protected DapperRepository(DapperContext context)
        {
            _context = context;
        }

        protected async Task WithConnectionAsync(
            Func<IDbConnection, Task> action)
        {
            using var connection = _context.CreateConnection();
            await action(connection);
        }

        protected async Task<T> WithConnectionAsync<T>(
            Func<IDbConnection, Task<T>> action)
        {
            using var connection = _context.CreateConnection();
            return await action(connection);
        }
    }
}
