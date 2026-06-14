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
            Func<IDbConnection, IDbTransaction?, Task> action,
            IUnitOfWork? uow = null)
        {
            if (uow != null)
            {
                await action(uow.Connection, uow.Transaction);
                return;
            }

            using var connection = _context.CreateConnection();
            await action(connection, null);
        }

        protected async Task<T> WithConnectionAsync<T>(
            Func<IDbConnection, IDbTransaction?, Task<T>> action,
            IUnitOfWork? uow = null)
        {
            if (uow != null)
            {
                return await action(uow.Connection, uow.Transaction);
            }

            using var connection = _context.CreateConnection();
            return await action(connection, null);
        }
    }
}
