using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class BaggageRepository : DapperRepository, IBaggageRepository
    {
        public BaggageRepository(DapperContext context) : base(context)
        {
        }
        public async Task CreateBaggageBulk(List<BaggageEntity> baggages, IUnitOfWork? uow = null)
        {
            await WithConnectionAsync(async (connection, transaction) =>
            {
                var table = new DataTable();
                table.Columns.Add("PassengerId", typeof(int));
                table.Columns.Add("ReservationId", typeof(int));
                table.Columns.Add("Weight", typeof(decimal));
                table.Columns.Add("Size", typeof(string));
                table.Columns.Add("Type", typeof(string));

                foreach (var b in baggages)
                {
                    table.Rows.Add(b.PassengerId, b.ReservationId, b.Weight, b.Size, b.Type);
                }
                var parameters = new DynamicParameters();
                parameters.Add("Baggages", table.AsTableValuedParameter("dbo.BaggageBulkType"));

                await connection.ExecuteAsync(
                    "dbo.sp_BulkBaggage",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );
            }, uow);
        }
    }
}
