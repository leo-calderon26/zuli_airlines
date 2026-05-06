using System;
using System.Threading.Tasks;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly DapperContext _context;

        public ServiceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateService(ServiceEntity service)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
                INSERT INTO Service (FlightId, Description)
                VALUES (@FlightId, @Description)";

            return await connection.ExecuteAsync(sql, new
            {
                service.FlightId,
                service.Description
            });
        }

        public async Task<ServiceEntity?> GetServiceByFlightId(Guid flightId)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
                SELECT Id, FlightId, Description
                FROM Service
                WHERE FlightId = @FlightId";

            return await connection.QueryFirstOrDefaultAsync<ServiceEntity>(sql, new { FlightId = flightId });
        }
    }
}
