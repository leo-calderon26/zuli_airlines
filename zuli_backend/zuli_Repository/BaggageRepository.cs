using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.DTO;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class BaggageRepository : DapperRepository, IBaggageRepository
    {
        private const int MAX_CHECKED_BAGS_PER_PASSANGER = 5;
        private readonly DapperContext _context;

        public BaggageRepository(DapperContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateBaggageBulk(List<BaggageEntity> baggages)
        {
            await WithConnectionAsync(async (connection) =>
            {
                var table = BuildBaggageDataTable(baggages);
                var parameters = new DynamicParameters();
                parameters.Add("Baggages", table.AsTableValuedParameter("dbo.BaggageBulkType"));

                await connection.ExecuteAsync(
                    "dbo.sp_BulkBaggage",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            });
        }

        public async Task AddAdditionalBaggageTransactional(string reservationCode, List<BaggageEntity> baggages)
        {
            await using var connection = _context.CreateConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            try
            {
                var reservationId = await connection.QuerySingleOrDefaultAsync<int>(
                    @"
                    SELECT ReservationId
                    FROM dbo.Reservation
                    WHERE ReservationCode = @reservationCode;
                    ",
                    new { reservationCode },
                    transaction
                );

                if (reservationId == 0)
                {
                    throw new ZuliNotFoundException("No se encontró la reserva.");
                }

                var passengerIds = baggages.Select(b => b.PassengerId).Distinct().ToList();
                await ValidatePassengersBelongToReservation(connection, transaction, reservationId, passengerIds);

                var checkedBagsByPassenger = baggages
                    .Where(b => b.Type?.Equals("Maleta", StringComparison.OrdinalIgnoreCase) == true)
                    .GroupBy(b => b.PassengerId)
                    .Select(g => new PassengerBaggageCountDTO(g.Key, g.Count()))
                    .ToList();

                var carryOnsByPassenger = baggages
                    .Where(b => b.Type?.Equals("Mano", StringComparison.OrdinalIgnoreCase) == true)
                    .GroupBy(b => b.PassengerId)
                    .Select(g => new PassengerBaggageCountDTO(g.Key, g.Count()))
                    .ToList();

                var existingCheckedBagsByPassenger = await ValidateCheckedBaggageLimit(
                    connection,
                    transaction,
                    reservationId,
                    checkedBagsByPassenger
                );

                await ValidateCarryOnLimit(connection, transaction, reservationId, carryOnsByPassenger);

                var requestedCheckedWeight = baggages
                    .Where(b => b.Type?.Equals("Maleta", StringComparison.OrdinalIgnoreCase) == true)
                    .Sum(b => b.Weight);

                if (requestedCheckedWeight > 0)
                {
                    await ValidateFlightBaggageCapacity(
                        connection,
                        transaction,
                        reservationId,
                        requestedCheckedWeight
                    );
                }

                var additionalBaggageTotal = await CalculateAdditionalBaggageTotal(
                    connection,
                    transaction,
                    reservationId,
                    checkedBagsByPassenger,
                    carryOnsByPassenger,
                    existingCheckedBagsByPassenger
                );

                await connection.ExecuteAsync(
                    @"
                    UPDATE dbo.Reservation
                    SET TotalPayment = ISNULL(TotalPayment, 0) + @additionalBaggageTotal
                    WHERE ReservationId = @reservationId;
                    ",
                    new { reservationId, additionalBaggageTotal },
                    transaction
                );

                var table = BuildBaggageDataTable(baggages, reservationId);
                var parameters = new DynamicParameters();
                parameters.Add("Baggages", table.AsTableValuedParameter("dbo.BaggageBulkType"));

                await connection.ExecuteAsync(
                    @"
                    INSERT INTO dbo.Baggage (PassengerId, ReservationId, Weight, Size, Type)
                    SELECT PassengerId, ReservationId, Weight, Size, Type
                    FROM @Baggages;
                    ",
                    parameters,
                    transaction
                );

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static DataTable BuildBaggageDataTable(List<BaggageEntity> baggages, int? reservationId = null)
        {
            var table = new DataTable();
            table.Columns.Add("PassengerId", typeof(int));
            table.Columns.Add("ReservationId", typeof(int));
            table.Columns.Add("Weight", typeof(decimal));
            table.Columns.Add("Size", typeof(string));
            table.Columns.Add("Type", typeof(string));

            foreach (var baggage in baggages)
            {
                table.Rows.Add(
                    baggage.PassengerId,
                    reservationId ?? baggage.ReservationId,
                    baggage.Weight,
                    baggage.Size,
                    baggage.Type
                );
            }

            return table;
        }

        private static async Task ValidatePassengersBelongToReservation(
            IDbConnection connection,
            IDbTransaction transaction,
            int reservationId,
            List<int> passengerIds)
        {
            var validPassengerCount = await connection.QuerySingleAsync<int>(
                @"
                SELECT COUNT(DISTINCT pr.PassengerId)
                FROM dbo.PassengerReservation pr
                WHERE pr.ReservationId = @reservationId
                AND pr.PassengerId IN @passengerIds;
                ",
                new { reservationId, passengerIds },
                transaction
            );

            if (validPassengerCount != passengerIds.Count)
            {
                throw new ZuliValidationException("baggage", "Algunos pasajeros no pertenecen a la reserva.");
            }
        }

        private static async Task<Dictionary<int, int>> ValidateCheckedBaggageLimit(
            IDbConnection connection,
            IDbTransaction transaction,
            int reservationId,
            List<PassengerBaggageCountDTO> checkedBagsByPassenger)
        {
            var existingCheckedBagsByPassenger = new Dictionary<int, int>();

            foreach (var checkedBagGroup in checkedBagsByPassenger)
            {
                var existingCheckedBags = await connection.QuerySingleAsync<int>(
                    @"
                    SELECT COUNT(*)
                    FROM dbo.Baggage b WITH (UPDLOCK, HOLDLOCK)
                    WHERE b.ReservationId = @reservationId
                    AND b.PassengerId = @passengerId
                    AND (
                        LOWER(ISNULL(b.Type, '')) LIKE '%checked%'
                        OR LOWER(ISNULL(b.Type, '')) LIKE '%fact%'
                        OR LOWER(ISNULL(b.Type, '')) LIKE '%maleta%'
                        OR LOWER(ISNULL(b.Type, '')) LIKE '%document%'
                    );
                    ",
                    new { reservationId, passengerId = checkedBagGroup.PassengerId },
                    transaction
                );

                existingCheckedBagsByPassenger[checkedBagGroup.PassengerId] = existingCheckedBags;

                if (existingCheckedBags + checkedBagGroup.Count > MAX_CHECKED_BAGS_PER_PASSANGER)
                {
                    throw new ZuliValidationException("baggage", "Cada pasajero puede llevar máximo 5 maletas documentadas.");
                }
            }

            return existingCheckedBagsByPassenger;
        }

        private static async Task ValidateCarryOnLimit(
            IDbConnection connection,
            IDbTransaction transaction,
            int reservationId,
            List<PassengerBaggageCountDTO> carryOnsByPassenger)
        {
            foreach (var carryOnGroup in carryOnsByPassenger)
            {
                var existingCarryOns = await connection.QuerySingleAsync<int>(
                    @"
                    SELECT COUNT(*)
                    FROM dbo.Baggage b WITH (UPDLOCK, HOLDLOCK)
                    WHERE b.ReservationId = @reservationId
                    AND b.PassengerId = @passengerId
                    AND (
                        LOWER(ISNULL(b.Type, '')) LIKE '%carry%'
                        OR LOWER(ISNULL(b.Type, '')) LIKE '%mano%'
                    );
                    ",
                    new { reservationId, passengerId = carryOnGroup.PassengerId },
                    transaction
                );

                if (existingCarryOns + carryOnGroup.Count > 1)
                {
                    throw new ZuliValidationException("baggage", "Cada pasajero puede llevar máximo un equipaje de mano.");
                }
            }
        }

        private static async Task ValidateFlightBaggageCapacity(
            IDbConnection connection,
            IDbTransaction transaction,
            int reservationId,
            decimal requestedCheckedWeight)
        {
            var insufficientFlights = await connection.QueryAsync<InsufficientBaggageFlightDTO>(
                @"
                WITH ReservationFlights AS (
                    SELECT DISTINCT f.Id, f.AircraftId
                    FROM dbo.Reservation r
                    INNER JOIN dbo.BoardingPass bp
                        ON r.ReservationCode = bp.ReservationCode
                    INNER JOIN dbo.Flight f WITH (UPDLOCK, HOLDLOCK)
                        ON bp.FlightId = f.Id
                    WHERE r.ReservationId = @reservationId
                )
                SELECT
                    f.Id AS FlightId,
                    CAST(a.BaggageCapacity AS DECIMAL(18, 2)) AS BaggageCapacity,
                    ISNULL(SUM(
                        CASE
                            WHEN LOWER(ISNULL(b.Type, '')) LIKE '%checked%'
                              OR LOWER(ISNULL(b.Type, '')) LIKE '%fact%'
                              OR LOWER(ISNULL(b.Type, '')) LIKE '%maleta%'
                              OR LOWER(ISNULL(b.Type, '')) LIKE '%document%'
                            THEN ISNULL(b.Weight, 0)
                            ELSE 0
                        END
                    ), 0) AS UsedBaggageWeight
                FROM ReservationFlights f
                INNER JOIN dbo.Aircraft a
                    ON f.AircraftId = a.AircraftId
                LEFT JOIN dbo.BoardingPass allBp
                    ON f.Id = allBp.FlightId
                LEFT JOIN dbo.Reservation allR
                    ON allBp.ReservationCode = allR.ReservationCode
                LEFT JOIN dbo.Baggage b
                    ON allR.ReservationId = b.ReservationId
                    AND allBp.PassengerId = b.PassengerId
                GROUP BY f.Id, a.BaggageCapacity
                HAVING ISNULL(SUM(
                    CASE
                        WHEN LOWER(ISNULL(b.Type, '')) LIKE '%checked%'
                          OR LOWER(ISNULL(b.Type, '')) LIKE '%fact%'
                          OR LOWER(ISNULL(b.Type, '')) LIKE '%maleta%'
                          OR LOWER(ISNULL(b.Type, '')) LIKE '%document%'
                        THEN ISNULL(b.Weight, 0)
                        ELSE 0
                    END
                ), 0) + @requestedCheckedWeight > CAST(a.BaggageCapacity AS DECIMAL(18, 2));
                ",
                new { reservationId, requestedCheckedWeight },
                transaction
            );

            var insufficientFlight = insufficientFlights.FirstOrDefault();
            if (insufficientFlight != null)
            {
                var availableWeight = insufficientFlight.BaggageCapacity - insufficientFlight.UsedBaggageWeight;
                throw new ZuliValidationException(
                    "baggage",
                    $"Capacidad de equipaje insuficiente en el vuelo {insufficientFlight.FlightId}. Disponible: {availableWeight:0.##} kg, solicitado: {requestedCheckedWeight:0.##} kg."
                );
            }
        }

        private static async Task<decimal> CalculateAdditionalBaggageTotal(
            IDbConnection connection,
            IDbTransaction transaction,
            int reservationId,
            List<PassengerBaggageCountDTO> checkedBagsByPassenger,
            List<PassengerBaggageCountDTO> carryOnsByPassenger,
            Dictionary<int, int> existingCheckedBagsByPassenger)
        {
            var flightPrices = (await connection.QueryAsync<AdditionalBaggageFlightPriceDTO>(
                @"
                SELECT DISTINCT
                    ISNULL(fr.CheckedPrice, 0) AS CheckedPrice,
                    ISNULL(fr.CarryOnPrice, 0) AS CarryOnPrice,
                    CASE
                        WHEN ISNULL(fr.CheckedBagMultiplier, 0) <= 0 THEN 1
                        ELSE fr.CheckedBagMultiplier
                    END AS CheckedBagMultiplier
                FROM dbo.Reservation r
                INNER JOIN dbo.BoardingPass bp
                    ON r.ReservationCode = bp.ReservationCode
                INNER JOIN dbo.Flight f
                    ON bp.FlightId = f.Id
                INNER JOIN dbo.FlightRoute fr
                    ON f.FlightRouteId = fr.FlightRouteId
                WHERE r.ReservationId = @reservationId;
                ",
                new { reservationId },
                transaction
            )).ToList();

            decimal total = 0;

            foreach (var checkedBagGroup in checkedBagsByPassenger)
            {
                var existingCheckedBags = existingCheckedBagsByPassenger[checkedBagGroup.PassengerId];
                for (int i = 1; i <= checkedBagGroup.Count; i++)
                {
                    var bagNumber = existingCheckedBags + i;
                    total += flightPrices.Sum(flight =>
                        flight.CheckedPrice * (decimal)Math.Pow((double)flight.CheckedBagMultiplier, bagNumber - 1)
                    );
                }
            }

            var additionalCarryOnCount = carryOnsByPassenger.Sum(group => group.Count);
            if (additionalCarryOnCount > 0)
            {
                total += additionalCarryOnCount * flightPrices.Sum(flight => flight.CarryOnPrice);
            }

            return total;
        }

    }
}
