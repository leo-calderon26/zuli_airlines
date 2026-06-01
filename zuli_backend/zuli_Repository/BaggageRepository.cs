using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class BaggageRepository : IBaggageRepository
    {
        private readonly DapperContext _context;

        public BaggageRepository(DapperContext context) => _context = context;

        public async Task CreateBaggage(BaggageEntity baggage)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Baggage (PassengerId, ReservationId, Weight, Size, Type)
                VALUES (@PassengerId, @ReservationId, @Weight, @Size, @Type);";

            await connection.ExecuteAsync(sql, new
            {
                baggage.PassengerId,
                baggage.ReservationId,
                baggage.Weight,
                baggage.Size,
                baggage.Type
            });
        }
    }
}
