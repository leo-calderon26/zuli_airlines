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
        public async Task<bool> PassengerExistsInFlights(
            IEnumerable<Guid> flightIds,
            string firstName,
            string firstLastName,
            string secondLastName,
            string birthDate,
            string passportCountry)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT TOP 1 1
                FROM dbo.BoardingPass bp
                INNER JOIN dbo.Person p
                    ON bp.PassengerId = p.PersonId
                LEFT JOIN dbo.Passport passport
                    ON p.PersonId = passport.PassengerId
                WHERE bp.FlightId IN @FlightIds
                AND LOWER(LTRIM(RTRIM(p.FirstName))) = LOWER(LTRIM(RTRIM(@FirstName)))
                AND LOWER(LTRIM(RTRIM(p.FirstLastName))) = LOWER(LTRIM(RTRIM(@FirstLastName)))
                AND LOWER(LTRIM(RTRIM(p.SecondLastName))) = LOWER(LTRIM(RTRIM(@SecondLastName)))
                AND CAST(p.BirthDate AS DATE) = CAST(@BirthDate AS DATE)
                AND LOWER(LTRIM(RTRIM(ISNULL(passport.PassportCountry, '')))) = LOWER(LTRIM(RTRIM(@PassportCountry)));
            ";

            var exists = await connection.QueryFirstOrDefaultAsync<int>(
                sql,
                new
                {
                    FlightIds = flightIds.ToList(),
                    FirstName = firstName,
                    FirstLastName = firstLastName,
                    SecondLastName = secondLastName,
                    BirthDate = birthDate,
                    PassportCountry = passportCountry
                }
            );

            return exists == 1;
        }
    }
}
