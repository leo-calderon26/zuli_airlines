using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DapperContext _context;

        public TicketRepository(DapperContext context) => _context = context;

        public async Task<ReservationEntity?> GetReservationByCode(string code)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT ReservationId, ReservationCode, ReservationOrigin, TotalPayment,
                       PurchaseDate, ClientId
                FROM Reservation
                WHERE ReservationCode = @Code";

            return await connection.QueryFirstOrDefaultAsync<ReservationEntity>(sql, new { Code = code });
        }

        public async Task<IEnumerable<PassengerReservationEntity>> GetPassengerReservations(int reservationId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT PassengerId, ReservationId FROM PassengerReservation WHERE ReservationId = @ReservationId";
            return await connection.QueryAsync<PassengerReservationEntity>(sql, new { ReservationId = reservationId });
        }

        public async Task<PersonEntity?> GetPersonById(int personId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT p.PersonId, p.FirstName, p.FirstLastName, p.SecondLastName, pe.Email
                FROM Person p
                LEFT JOIN PersonEmail pe ON p.PersonId = pe.PersonId
                WHERE p.PersonId = @PersonId";

            return await connection.QueryFirstOrDefaultAsync<PersonEntity>(sql, new { PersonId = personId });
        }
    }
}
