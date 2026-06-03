using Dapper;
using System.Data;
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

        public async Task<PurchaseConfirmationEntity?> GetPurchaseConfirmationAsync(string reservationCode)
        {
            using var connection = _context.CreateConnection();

            var confirmation = await GetReservationAsync(connection, reservationCode);

            if (confirmation == null)
            {
                return null;
            }

            confirmation.Passengers = await GetPassengersAsync(connection, reservationCode);
            confirmation.Flights = await GetFlightsAsync(connection, reservationCode);

            return confirmation;
        }

        private static async Task<PurchaseConfirmationEntity?> GetReservationAsync(
            IDbConnection connection,
            string reservationCode)
        {
            const string query = @"
                SELECT
                    r.ReservationId,
                    r.ReservationCode,
                    CONCAT(
                        buyerPerson.FirstName,
                        ' ',
                        buyerPerson.FirstLastName,
                        ' ',
                        buyerPerson.SecondLastName
                    ) AS BuyerName,
                    buyerEmail.Email AS BuyerEmail,
                    ISNULL(b.Phone, '') AS BuyerPhone,
                    ISNULL(r.PaymentMethod, '') AS PaymentMethod,
                    ISNULL(r.FlightClass, '') AS FlightClass,
                    ISNULL(r.TotalPayment, 0) AS TotalAmount
                FROM dbo.Reservation r
                INNER JOIN dbo.Buyer b
                    ON r.BuyerId = b.BuyerId
                INNER JOIN dbo.Person buyerPerson
                    ON b.PersonId = buyerPerson.PersonId
                LEFT JOIN dbo.PersonEmail buyerEmail
                    ON buyerPerson.PersonId = buyerEmail.PersonId
                WHERE r.ReservationCode = @reservationCode;
            ";

            return await connection.QueryFirstOrDefaultAsync<PurchaseConfirmationEntity>(
                query,
                new { reservationCode }
            );
        }

        private static async Task<List<PurchaseConfirmationPassengerEntity>> GetPassengersAsync(
            IDbConnection connection,
            string reservationCode)
        {
            const string query = @"
                SELECT
                    CONCAT(
                        passengerPerson.FirstName,
                        ' ',
                        passengerPerson.FirstLastName,
                        ' ',
                        passengerPerson.SecondLastName
                    ) AS FullName,
                    passengerPerson.BirthDate,
                    passengerPerson.Gender,
                    ISNULL(passport.PassportCountry, '') AS PassportCountry,
                    ISNULL(baggageSummary.CheckedBaggageQuantity, 0) AS CheckedBaggageQuantity,
                    ISNULL(baggageSummary.CarryOnQuantity, 0) AS CarryOnQuantity
                FROM dbo.Reservation r
                INNER JOIN dbo.PassengerReservation pr
                    ON r.ReservationId = pr.ReservationId
                INNER JOIN dbo.Person passengerPerson
                    ON pr.PassengerId = passengerPerson.PersonId
                LEFT JOIN dbo.Passport passport
                    ON passengerPerson.PersonId = passport.PassengerId
                LEFT JOIN (
                    SELECT
                        PassengerId,
                        ReservationId,
                        SUM(
                            CASE
                                WHEN LOWER(ISNULL(Type, '')) LIKE '%checked%'
                                  OR LOWER(ISNULL(Type, '')) LIKE '%fact%'
                                  OR LOWER(ISNULL(Type, '')) LIKE '%maleta%'
                                  OR LOWER(ISNULL(Type, '')) LIKE '%document%'
                                THEN 1
                                ELSE 0
                            END
                        ) AS CheckedBaggageQuantity,
                        SUM(
                            CASE
                                WHEN LOWER(ISNULL(Type, '')) LIKE '%carry%'
                                  OR LOWER(ISNULL(Type, '')) LIKE '%mano%'
                                THEN 1
                                ELSE 0
                            END
                        ) AS CarryOnQuantity
                    FROM dbo.Baggage
                    GROUP BY PassengerId, ReservationId
                ) baggageSummary
                    ON pr.PassengerId = baggageSummary.PassengerId
                    AND pr.ReservationId = baggageSummary.ReservationId
                WHERE r.ReservationCode = @reservationCode
                ORDER BY passengerPerson.FirstName, passengerPerson.FirstLastName;
            ";

            var passengers = await connection.QueryAsync<PurchaseConfirmationPassengerEntity>(
                query,
                new { reservationCode }
            );

            return passengers.ToList();
        }

        private static async Task<List<PurchaseConfirmationFlightEntity>> GetFlightsAsync(
            IDbConnection connection,
            string reservationCode)
        {
            const string query = @"
                SELECT DISTINCT
                    f.Id AS FlightId,
                    CONCAT('ZU-',fr.FlightRouteId) AS FlightNumber,
                    airline.AirlineName,
                    departureAirport.Name AS OriginAirportName,
                    departureAirport.AirportCode AS OriginAirportCode,
                    arrivalAirport.Name AS DestinationAirportName,
                    arrivalAirport.AirportCode AS DestinationAirportCode,
                    ISNULL(f.RealDepartureTime, f.FlightDate) AS DepartureDateTime,
                    ISNULL(f.RealArrivalTime, DATEADD(MINUTE, f.Duration, f.FlightDate)) AS ArrivalDateTime
                FROM dbo.Reservation r
                INNER JOIN dbo.BoardingPass bp
                    ON r.ReservationCode = bp.ReservationCode
                INNER JOIN dbo.Flight f
                    ON bp.FlightId = f.Id
                INNER JOIN dbo.FlightRoute fr
                    ON f.FlightRouteId = fr.FlightRouteId
                INNER JOIN dbo.Airline airline
                    ON fr.AirlineId = airline.AirlineId
                INNER JOIN dbo.Airport departureAirport
                    ON fr.DepartureAirport = departureAirport.AirportCode
                INNER JOIN dbo.Airport arrivalAirport
                    ON fr.ArrivalAirport = arrivalAirport.AirportCode
                WHERE r.ReservationCode = @reservationCode
                ORDER BY DepartureDateTime;
            ";

            var flights = await connection.QueryAsync<PurchaseConfirmationFlightEntity>(
                query,
                new { reservationCode }
            );

            return flights.ToList();
        }
    }
}