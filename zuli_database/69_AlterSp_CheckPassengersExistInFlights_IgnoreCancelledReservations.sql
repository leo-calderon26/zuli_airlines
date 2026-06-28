USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_CheckPassengersExistInFlights
    @Passengers dbo.PassengerCheckBulkType READONLY,
    @FlightIds dbo.GuidList READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        passenger.PassengerIndex
    FROM @Passengers passenger
    WHERE EXISTS
    (
        SELECT 1
        FROM dbo.BoardingPass boardingPass
        INNER JOIN dbo.Reservation reservation
            ON reservation.ReservationCode = boardingPass.ReservationCode
            AND reservation.ReservationStatusId = 1
        INNER JOIN dbo.Person person
            ON person.PersonId = boardingPass.PassengerId
        LEFT JOIN dbo.Passport passport
            ON passport.PassengerId = person.PersonId
        INNER JOIN @FlightIds flight
            ON flight.Id = boardingPass.FlightId
        WHERE LOWER(LTRIM(RTRIM(person.FirstName))) =
              LOWER(LTRIM(RTRIM(passenger.FirstName)))
          AND LOWER(LTRIM(RTRIM(person.FirstLastName))) =
              LOWER(LTRIM(RTRIM(passenger.FirstLastName)))
          AND LOWER(LTRIM(RTRIM(person.SecondLastName))) =
              LOWER(LTRIM(RTRIM(ISNULL(passenger.SecondLastName, ''))))
          AND TRY_CONVERT(DATE, person.BirthDate, 23) =
              TRY_CONVERT(DATE, passenger.BirthDate, 23)
          AND LOWER(LTRIM(RTRIM(ISNULL(passport.PassportCountry, '')))) =
              LOWER(LTRIM(RTRIM(passenger.PassportCountry)))
    );
END;
GO