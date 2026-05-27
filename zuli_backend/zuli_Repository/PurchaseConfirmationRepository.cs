using Dapper;
using zuli_Business.DTO;
using zuli_Data.Context;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class PurchaseConfirmationRepository : IPurchaseConfirmationRepository
    {
        private readonly DapperContext _context;

        public PurchaseConfirmationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<PurchaseConfirmationPageDTO?> GetPurchaseConfirmationAsync(Guid reservationId)
        {
            const string reservationQuery = @"
                SELECT
                    r.ReservationId,
                    r.ReservationCode,
                    r.BuyerName,
                    r.BuyerEmail,
                    r.BuyerPhone,
                    r.PaymentMethod,
                    r.FlightClass,
                    r.TotalAmount
                FROM Reservation r
                WHERE r.ReservationId = @reservationId;
            ";

            const string passengersQuery = @"
                SELECT
                    CONCAT(p.FirstName, ' ', p.FirstLastName, ' ', p.SecondLastName) AS FullName,
                    p.BirthDate,
                    p.Gender,
                    pp.Country AS PassportCountry,
                    ISNULL(b.CheckedBaggageQuantity, 0) AS CheckedBaggageQuantity,
                    ISNULL(b.CarryOnQuantity, 0) AS CarryOnQuantity
                FROM PassengerReservation pr
                INNER JOIN Person p
                    ON pr.PersonId = p.PersonId
                LEFT JOIN Passport pp
                    ON p.PersonId = pp.PersonId
                LEFT JOIN Baggage b
                    ON pr.PassengerReservationId = b.PassengerReservationId
                WHERE pr.ReservationId = @reservationId;
            ";

            const string flightsQuery = @"
                SELECT
                    f.FlightId,
                    CAST(f.FlightId AS VARCHAR(20)) AS FlightNumber,
                    al.Name AS AirlineName,
                    origin.Name AS OriginAirportName,
                    origin.Code AS OriginAirportCode,
                    destination.Name AS DestinationAirportName,
                    destination.Code AS DestinationAirportCode,
                    f.DepartureDateTime,
                    f.ArrivalDateTime
                FROM Itinerary i
                INNER JOIN Flight f
                    ON i.FlightId = f.FlightId
                INNER JOIN FlightRoute fr
                    ON f.FlightRouteId = fr.FlightRouteId
                INNER JOIN Airport origin
                    ON fr.OriginAirportId = origin.AirportId
                INNER JOIN Airport destination
                    ON fr.DestinationAirportId = destination.AirportId
                INNER JOIN Airline al
                    ON f.AirlineId = al.AirlineId
                WHERE i.ReservationId = @reservationId
                ORDER BY f.DepartureDateTime;
            ";

            using var connection = _context.CreateConnection();

            var confirmation = await connection.QueryFirstOrDefaultAsync<PurchaseConfirmationPageDTO>(
                reservationQuery,
                new { reservationId }
            );

            if (confirmation is null)
            {
                return null;
            }

            var passengers = await connection.QueryAsync<PurchaseConfirmationPassengerDTO>(
                passengersQuery,
                new { reservationId }
            );

            var flights = await connection.QueryAsync<PurchaseConfirmationFlightDTO>(
                flightsQuery,
                new { reservationId }
            );

            confirmation.Passengers = passengers.ToList();
            confirmation.Flights = flights.ToList();

            return confirmation;
        }
    }
}