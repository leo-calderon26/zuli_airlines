using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly DapperContext _context;

        public BuyerRepository(DapperContext context) => _context = context;

        public async Task<int> CreateBuyer(BuyerEntity buyer)
        {
            using var connection = _context.CreateConnection();

            var buyerId = await connection.QuerySingleAsync<int>(
                "sp_UpsertBuyer",
                new
                {
                    buyer.FirstName,
                    buyer.FirstLastName,
                    buyer.SecondLastName,
                    buyer.BirthDate,
                    buyer.Email,
                    buyer.Phone
                },
                commandType: System.Data.CommandType.StoredProcedure);

            return buyerId;
        }
    }
}
