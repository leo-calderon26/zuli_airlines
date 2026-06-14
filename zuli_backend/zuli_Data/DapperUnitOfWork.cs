using System.Data;
using Microsoft.Data.SqlClient;

namespace zuli_Data
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly SqlConnection _connection;
        private IDbTransaction? _transaction;

        public IDbConnection Connection => _connection;
        public IDbTransaction? Transaction => _transaction;

        public DapperUnitOfWork(DapperContext context)
        {
            _connection = context.CreateConnection();
        }

        public Task BeginTransactionAsync()
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            _transaction = _connection.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            _transaction?.Commit();
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            _transaction?.Rollback();
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
