using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class ReservationRepository : DapperRepository, IReservationRepository
    {
        public ReservationRepository(DapperContext context) : base(context)
        {
        }

        public async Task<int> CreateReservation(ReservationEntity reservation, IUnitOfWork? uow = null)
        {
            return await WithConnectionAsync(async (connection, transaction) =>
            {
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
                }, transaction);
            }, uow);
        }

        public async Task<int> CreateBoardingPassesBulk(List<BoardingPassEntity> boardingPasses, IUnitOfWork? uow = null)
        {
            return await WithConnectionAsync(async (connection, transaction) =>
            {
                var table = new DataTable();
                table.Columns.Add("FlightId", typeof(Guid));
                table.Columns.Add("ReservationCode", typeof(string));
                table.Columns.Add("PassengerId", typeof(int));

                foreach (var pass in boardingPasses)
                {
                    table.Rows.Add(pass.FlightId, pass.ReservationCode, pass.PassengerId);
                }

                var parameters = new DynamicParameters();
                parameters.Add("BoardingPasses", table.AsTableValuedParameter("dbo.BoardingPassBulkType"));

                var result = await connection.ExecuteAsync(
                    "dbo.sp_BulkBoardingPass",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }, uow);
        }

        public async Task<bool> ReservationCodeExists(string reservationCode, IUnitOfWork? uow = null)
        {
            return await WithConnectionAsync(async (connection, transaction) =>
            {
                var sql = "SELECT TOP 1 1 FROM dbo.Reservation WHERE ReservationCode = @ReservationCode;";
                var exists = await connection.QueryFirstOrDefaultAsync<int>(
                    sql,
                    new { ReservationCode = reservationCode },
                    transaction);

                return exists == 1;
            }, uow);
        }

        public async Task<int> CreatePassengerReservationsBulk(List<PassengerReservationEntity> prs, IUnitOfWork? uow = null)
        {
            return await WithConnectionAsync(async (connection, transaction) =>
            {
                var table = new DataTable();
                table.Columns.Add("PassengerId", typeof(int));
                table.Columns.Add("ReservationId", typeof(int));
                foreach (var pr in prs)
                {
                    table.Rows.Add(pr.PassengerId, pr.ReservationId);
                }

                var parameters = new DynamicParameters();
                parameters.Add("PassengerReservations", table.AsTableValuedParameter("dbo.PassengerReservationBulkType"));

                var result = await connection.QueryAsync<int>(
                    "dbo.sp_BulkPassengerReservation",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure
                );
                return result.Count();
            }, uow);
        }

        public async Task<bool> PassengerExistsInFlights(
            IEnumerable<Guid> flightIds,
            string firstName,
            string firstLastName,
            string secondLastName,
            string birthDate,
            string passportCountry)
        {
            return await WithConnectionAsync(async (connection, _) =>
            {
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
            });
        }

        public async Task<Dictionary<int, bool>> PassengersExistInFlights(
            IEnumerable<Guid> flightIds,
            List<PassengerCheckInfo> passengers)
        {
            return await WithConnectionAsync(async (connection, _) =>
            {
                var flightIdList = flightIds.ToList();

                if (passengers.Count == 0)
                {
                    return new Dictionary<int, bool>();
                }

                var passengersTable = new DataTable();
                passengersTable.Columns.Add("PassengerIndex", typeof(int));
                passengersTable.Columns.Add("FirstName", typeof(string));
                passengersTable.Columns.Add("FirstLastName", typeof(string));
                passengersTable.Columns.Add("SecondLastName", typeof(string));
                passengersTable.Columns.Add("BirthDate", typeof(string));
                passengersTable.Columns.Add("PassportCountry", typeof(string));

                foreach (var p in passengers)
                {
                    passengersTable.Rows.Add(
                        p.Index,
                        p.FirstName,
                        p.FirstLastName,
                        p.SecondLastName,
                        p.BirthDate,
                        p.PassportCountry
                    );
                }

                var flightIdsTable = new DataTable();
                flightIdsTable.Columns.Add("Id", typeof(Guid));
                foreach (var id in flightIdList)
                {
                    flightIdsTable.Rows.Add(id);
                }

                var parameters = new DynamicParameters();
                parameters.Add("Passengers", passengersTable.AsTableValuedParameter("dbo.PassengerCheckBulkType"));
                parameters.Add("FlightIds", flightIdsTable.AsTableValuedParameter("dbo.GuidList"));

                var matchedIndices = await connection.QueryAsync<int>(
                    "dbo.sp_CheckPassengersExistInFlights",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var matched = matchedIndices.ToHashSet();
                var result = new Dictionary<int, bool>();

                for (int i = 0; i < passengers.Count; i++)
                {
                    result[passengers[i].Index] = matched.Contains(passengers[i].Index);
                }

                return result;
            });
        }
    }
}
