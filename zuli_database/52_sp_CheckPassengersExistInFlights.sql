USE ZuliAirlines;
GO

IF OBJECT_ID(N'dbo.sp_CheckPassengersExistInFlights', N'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_CheckPassengersExistInFlights;
END
GO

CREATE PROCEDURE dbo.sp_CheckPassengersExistInFlights
    @Passengers dbo.PassengerCheckBulkType READONLY,
    @FlightIds  dbo.GuidList                READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.PassengerIndex
    FROM @Passengers p
    WHERE EXISTS
    (
        SELECT 1
        FROM dbo.BoardingPass bp
        INNER JOIN dbo.Person per ON per.PersonId = bp.PassengerId
        LEFT JOIN dbo.Passport passport ON passport.PassengerId = per.PersonId
        INNER JOIN @FlightIds f ON f.Id = bp.FlightId
        WHERE
            LOWER(LTRIM(RTRIM(per.FirstName))) = LOWER(LTRIM(RTRIM(p.FirstName)))
            AND LOWER(LTRIM(RTRIM(per.FirstLastName))) = LOWER(LTRIM(RTRIM(p.FirstLastName)))
            AND LOWER(LTRIM(RTRIM(per.SecondLastName))) = LOWER(LTRIM(RTRIM(ISNULL(p.SecondLastName, ''))))
            AND TRY_CONVERT(DATE, per.BirthDate, 23) = TRY_CONVERT(DATE, p.BirthDate, 23)
            AND LOWER(LTRIM(RTRIM(ISNULL(passport.PassportCountry, '')))) = LOWER(LTRIM(RTRIM(p.PassportCountry)))
    );
END
