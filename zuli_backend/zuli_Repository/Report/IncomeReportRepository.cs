using System.Data;
using Dapper;
using zuli_Data;
using zuli_Repository.Interface.Reports;
using zuli_Data.Entities.Reports;

namespace zuli_Repository.Reports
{
    public class IncomeReportRepository : IIncomeReportRepository
    {
        private readonly DapperContext _context;
        public IncomeReportRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IncomeReportResultEntity> GetIncomeReportAsync(IncomeReportRequestEntity incomeReportRequestEntity)
        {
            const string sql = """
                SELECT * FROM dbo.GetIncomeReport(
                    @Year,
                    @Origin,
                    @Destination,
                    @AirlineId
                )
                """;

            using var connection = _context.CreateConnection();
            var rows = (await connection.QueryAsync<IncomeReportRowEntity>(
                sql,
                incomeReportRequestEntity,
                commandType: CommandType.Text
            )).ToList();

            var summary = new IncomeReportSummaryEntity
            {
                TotalFlights = rows.Sum(r => r.Flights),
                TotalFirstClass = rows.Sum(r => r.FirstClass),
                TotalTouristClass = rows.Sum(r => r.TouristClass),
                TotalPassengers = rows.Sum(r => r.TotalPassengers),
                TotalTicketIncome = rows.Sum(r => r.TicketIncome),
                TotalBaggageIncome = rows.Sum(r => r.BaggageIncome),
                TotalIncome = rows.Sum(r => r.TotalIncome)
            };

            return new IncomeReportResultEntity
            {
                Rows = rows,
                Summary = summary
            };
        }
    }
}

