using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FlightRepository : IFlightRepository
    {
        private readonly DapperContext _context;

        public FlightRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateFlight(FlightEntity flight)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                INSERT INTO Flight (
                    Id,
                    Status,
                    FlightDate,
                    TouristPrice,
                    FirstClassPrice,
                    RealDepartureTime,
                    RealArrivalTime,
                    CheckInStartTime,
                    CheckInDeadline,
                    AirlineId,
                    AircraftId,
                    ItineraryId,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    AdminId,
                    FlightRouteId
                ) VALUES (
                    @Id,
                    @Status,
                    @FlightDate,
                    @TouristPrice,
                    @FirstClassPrice,
                    @RealDepartureTime,
                    @RealArrivalTime,
                    @CheckInStartTime,
                    @CheckInDeadline,
                    @AirlineId,
                    @AircraftId,
                    @ItineraryId,
                    @Duration,
                    @CarryOnPrice,
                    @CheckedPrice,
                    @AvailableSeats,
                    @AdminId,
                    @FlightRouteId
                )";

            return await connection.ExecuteAsync(sql, new
            {
                flight.Id,
                flight.Status,
                flight.FlightDate,
                flight.TouristPrice,
                flight.FirstClassPrice,
                flight.RealDepartureTime,
                flight.RealArrivalTime,
                flight.CheckInStartTime,
                flight.CheckInDeadline,
                flight.AirlineId,
                flight.AircraftId,
                flight.ItineraryId,
                flight.Duration,
                flight.CarryOnPrice,
                flight.CheckedPrice,
                flight.AvailableSeats,
                flight.AdminId,
                flight.FlightRouteId
            });
        }

        public async Task<IEnumerable<FlightEntity>> GetAllFlights()
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    Id,
                    Status,
                    FlightDate,
                    TouristPrice,
                    FirstClassPrice,
                    RealDepartureTime,
                    RealArrivalTime,
                    CheckInStartTime,
                    CheckInDeadline,
                    AirlineId,
                    AircraftId,
                    ItineraryId,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    AdminId,
                    FlightRouteId
                FROM Flight";

            return await connection.QueryAsync<FlightEntity>(sql);
        }

        public async Task<FlightEntity> GetFlightById(Guid id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    Id,
                    Status,
                    FlightDate,
                    TouristPrice,
                    FirstClassPrice,
                    RealDepartureTime,
                    RealArrivalTime,
                    CheckInStartTime,
                    CheckInDeadline,
                    AirlineId,
                    AircraftId,
                    ItineraryId,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    AdminId,
                    FlightRouteId
                FROM Flight
                WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<FlightEntity>(sql, new { Id = id });
        }