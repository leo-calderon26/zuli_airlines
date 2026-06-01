using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DapperContext _context;

        public ReservationRepository(DapperContext context) => _context = context;

        public async Task<int> CreateReservation(ReservationEntity reservation)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Reservation (ReservationCode, ReservationOrigin, TotalPayment, PurchaseDate, BuyerId, FlightClass, PaymentMethod)
                VALUES (@ReservationCode, @ReservationOrigin, @TotalPayment, @PurchaseDate, @BuyerId, @FlightClass, @PaymentMethod);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                reservation.ReservationCode,
                reservation.ReservationOrigin,
                reservation.TotalPayment,
                reservation.PurchaseDate,
                reservation.BuyerId,
                reservation.FlightClass,
                reservation.PaymentMethod
            });
        }

        public async Task CreateBoardingPass(BoardingPassEntity boardingPass)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO BoardingPass (FlightId, ReservationCode, PassengerId)
                VALUES (@FlightId, @ReservationCode, @PassengerId);";

            await connection.ExecuteAsync(sql, new
            {
                boardingPass.FlightId,
                boardingPass.ReservationCode,
                boardingPass.PassengerId
            });
        }

        public async Task CreatePassengerReservation(PassengerReservationEntity pr)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO PassengerReservation (PassengerId, ReservationId)
                VALUES (@PassengerId, @ReservationId);";

            await connection.ExecuteAsync(sql, new
            {
                pr.PassengerId,
                pr.ReservationId
            });
        }
    }
}
