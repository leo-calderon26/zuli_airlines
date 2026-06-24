using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using zuli_Data;
using zuli_Data.Entities.Reports;
using zuli_Data.Entities.Filters;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FlightsReportRepository : IFlightsReportRepository
    {
        private readonly DapperContext _context;

        public FlightsReportRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FlightsReportEntity>> GetFlightsReportAsync(FlightsReportFilterEntity filters)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
            SELECT * FROM dbo.GetFlightsReport(
            @FromDate,
            @ToDate,
            @Origin,
            @Destination,
            @FlightClass
            );
            ";

            var report = await connection.QueryAsync<FlightsReportEntity>(sql, filters);

            return report;
        }
    }
}