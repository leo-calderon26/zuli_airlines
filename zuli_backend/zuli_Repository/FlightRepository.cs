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
                    FlightId,
                    aircraftId,
                    originAirport,
                    destinationAirport,
                    monday,
                    tuesday,
                    wednesday,
                    thursday,
                    friday,
                    saturday,
                    sunday,
                    departureTime,
                    arrivalTime,
                    duration,
                    firstClassPrice,
                    touristPrice,
                    carryOnPrice,
                    carryOnWeightKg,
                    checkedBaggagePrice,
                    checkedBaggageMaxWeightKg,
                    checkedBaggageMultiplierPercent,
                    availableSeats,
                    status,
                    createdAt
                ) VALUES (
                    @FlightId,
                    @aircraftId,
                    @originAirport,
                    @destinationAirport,
                    @monday,
                    @tuesday,
                    @wednesday,
                    @thursday,
                    @friday,
                    @saturday,
                    @sunday,
                    @departureTime,
                    @arrivalTime,
                    @duration,
                    @firstClassPrice,
                    @touristPrice,
                    @carryOnPrice,
                    @carryOnWeightKg,
                    @checkedBaggagePrice,
                    @checkedBaggageMaxWeightKg,
                    @checkedBaggageMultiplierPercent,
                    @availableSeats,
                    @status,
                    @createdAt
                )";

            return await connection.ExecuteAsync(sql, new
            {
                flight.FlightId,
                flight.aircraftId,
                flight.originAirport,
                flight.destinationAirport,
                flight.monday,
                flight.tuesday,
                flight.wednesday,
                flight.thursday,
                flight.friday,
                flight.saturday,
                flight.sunday,
                flight.departureTime,
                flight.arrivalTime,
                flight.duration,
                flight.firstClassPrice,
                flight.touristPrice,
                flight.carryOnPrice,
                flight.carryOnWeightKg,
                flight.checkedBaggagePrice,
                flight.checkedBaggageMaxWeightKg,
                flight.checkedBaggageMultiplierPercent,
                flight.availableSeats,
                flight.status,
                createdAt = DateTime.UtcNow
            });
        }
    }
}