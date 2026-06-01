using Dapper;
using zuli_Data;
using zuli_Data.Entities;
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

        public async Task<PurchaseConfirmationEntity?> GetPurchaseConfirmationAsync(Guid reservationId)
        {
            const string reservationQuery = @"
                SELECT
                    r.ReservationId,
                    r.ReservationCode,
                    CONCAT(
                        buyerPerson.FirstName,
                        ' ',
                        buyerPerson.FirstLastName,
                        ' ',
                        ISNULL(buyerPerson.SecondLastName, '')
                    ) AS BuyerName,
                    buyerEmail.Email AS BuyerEmail,
                    b.PhoneNumber AS BuyerPhone,
                    r.PaymentMethod,
                    r.FlightClass,
                    r.TotalAmount
                FROM Reservation r
                INNER JOIN Buyer b
                    ON r.BuyerId = b.BuyerId
                INNER JOIN Person buyerPerson
                    ON b.PersonId = buyerPerson.PersonId
                INNER JOIN EmailPerson buyerEmail
                    ON buyerPerson.PersonId = buyerEmail.PersonId
                WHERE r.ReservationId = @reservationId;
            ";

            const string passengersQuery = @"
                SELECT
                    CONCAT(
                        passengerPerson.FirstName,
                        ' ',
                        passengerPerson.FirstLastName,
                        ' ',
                        ISNULL(passengerPerson.SecondLastName, '')
                    ) AS FullName,
                    passengerPerson.BirthDate,
                    passengerPerson.Gender,
                    p.Country AS PassportCountry,
                    ISNULL(bg.CheckedBaggageQuantity, 0) AS CheckedBaggageQuantity,
                    ISNULL(bg.CarryOnQuantity, 0) AS CarryOnQuantity
                FROM PassengerReservation pr
                INNER JOIN Person passengerPerson
                    ON pr.PersonId = passengerPerson.PersonId
                LEFT JOIN Passport p
                    ON passengerPerson.PersonId = p.PersonId
                LEFT JOIN Baggage bg
                    ON pr.PassengerReservationId = bg.PassengerReservationId
                WHERE pr.ReservationId = @reservationId;
            ";

            const string flightsQuery = @"
                SELECT DISTINCT
                    f.FlightId,
                    CAST(f.FlightId AS VARCHAR(20)) AS FlightNumber,
                    al.Name AS AirlineName,
                    origin.Name AS OriginAirportName,
                    origin.Code AS OriginAirportCode,
                    destination.Name AS DestinationAirportName,
                    destination.Code AS DestinationAirportCode,
                    f.DepartureDateTime,
                    f.ArrivalDateTime
                FROM PassengerReservation pr
                INNER JOIN Flight f
                    ON pr.FlightId = f.FlightId
                INNER JOIN Airport origin
                    ON f.OriginAirportId = origin.AirportId
                INNER JOIN Airport destination
                    ON f.DestinationAirportId = destination.AirportId
                INNER JOIN Airline al
                    ON f.AirlineId = al.AirlineId
                WHERE pr.ReservationId = @reservationId
                ORDER BY f.DepartureDateTime;
            ";

            using var connection = _context.CreateConnection();

            var confirmation = await connection.QueryFirstOrDefaultAsync<PurchaseConfirmationEntity>(
                reservationQuery,
                new { reservationId }
            );

            if (confirmation == null)
            {
                return null;
            }

            var passengers = await connection.QueryAsync<PurchaseConfirmationPassengerEntity>(
                passengersQuery,
                new { reservationId }
            );

            var flights = await connection.QueryAsync<PurchaseConfirmationFlightEntity>(
                flightsQuery,
                new { reservationId }
            );

            confirmation.Passengers = passengers.ToList();
            confirmation.Flights = flights.ToList();

            return confirmation;
        }
    }
}