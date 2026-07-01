CREATE OR ALTER PROCEDURE sp_GetReservationSearchData
    @ReservationCode NVARCHAR(50),
    @LastNameSearch NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT 
        ISNULL(f.RealDepartureTime, CAST(CAST(f.FlightDate AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME)) AS DepartureDateTime,
        ISNULL(f.RealArrivalTime, 
            CASE 
                WHEN fr.ScheduledArrivalTime < fr.ScheduledDepartureTime 
                THEN DATEADD(day, 1, CAST(CAST(f.FlightDate AS DATE) AS DATETIME) + CAST(fr.ScheduledArrivalTime AS DATETIME))
                ELSE CAST(CAST(f.FlightDate AS DATE) AS DATETIME) + CAST(fr.ScheduledArrivalTime AS DATETIME)
            END
        ) AS ArrivalDateTime,
        aOrigin.City AS OriginCity,
        aOrigin.AirportCode AS OriginCode,
        aDest.City AS DestinationCity,
        aDest.AirportCode AS DestinationCode,
        r.FlightClass,
        al.AirlineName AS Airline,
        CONCAT('ZL ', fr.FlightRouteId) AS FlightNumber,
        ac.Model AS AircraftModel,
        r.ReservationStatusId,
        pe.Email AS BuyerEmail
    FROM Reservation r
    INNER JOIN BoardingPass bp ON r.ReservationCode = bp.ReservationCode
    INNER JOIN Flight f ON bp.FlightId = f.Id
    INNER JOIN FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
    INNER JOIN Airport aOrigin ON fr.DepartureAirport = aOrigin.AirportCode
    INNER JOIN Airport aDest ON fr.ArrivalAirport = aDest.AirportCode
    INNER JOIN Airline al ON fr.AirlineId = al.AirlineId
    INNER JOIN Aircraft ac ON fr.AircraftId = ac.AircraftId
    INNER JOIN Buyer b ON r.BuyerId = b.BuyerId
    INNER JOIN Person bp_person ON b.PersonId = bp_person.PersonId
    LEFT JOIN PersonEmail pe ON bp_person.PersonId = pe.PersonId
    WHERE r.ReservationCode = @ReservationCode 
    AND EXISTS (
        SELECT 1 
        FROM Person p
        LEFT JOIN Buyer b2 ON p.PersonId = b2.PersonId
        LEFT JOIN PassengerReservation pr ON p.PersonId = pr.PassengerId
        WHERE (b2.BuyerId = r.BuyerId OR pr.ReservationId = r.ReservationId)
        AND (
            LOWER(p.FirstLastName) LIKE @LastNameSearch OR 
            LOWER(p.SecondLastName) LIKE @LastNameSearch OR 
            LOWER(CONCAT(p.FirstLastName, ' ', p.SecondLastName)) LIKE @LastNameSearch
        )
    )
    ORDER BY DepartureDateTime ASC;

    SELECT 
        p.FirstName, 
        p.FirstLastName, 
        p.SecondLastName,
        ISNULL(SUM(CASE WHEN b.Type = 'Maleta' THEN 1 ELSE 0 END), 0) AS CheckedBaggageQuantity,
        ISNULL(SUM(CASE WHEN b.Type = 'Mano' THEN 1 ELSE 0 END), 0) AS CarryOnQuantity
    FROM dbo.PassengerReservation pr
    INNER JOIN dbo.Person p ON pr.PassengerId = p.PersonId
    INNER JOIN dbo.Reservation r ON pr.ReservationId = r.ReservationId
    LEFT JOIN dbo.Baggage b ON pr.PassengerId = b.PassengerId AND r.ReservationId = b.ReservationId
    WHERE r.ReservationCode = @ReservationCode
    GROUP BY p.FirstName, p.FirstLastName, p.SecondLastName;

END;