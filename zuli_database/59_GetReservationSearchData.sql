CREATE PROCEDURE sp_GetReservationSearchData
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
        ac.Model AS AircraftModel
    FROM Reservation r
    INNER JOIN BoardingPass bp ON r.ReservationCode = bp.ReservationCode
    INNER JOIN Flight f ON bp.FlightId = f.Id
    INNER JOIN FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
    INNER JOIN Airport aOrigin ON fr.DepartureAirport = aOrigin.AirportCode
    INNER JOIN Airport aDest ON fr.ArrivalAirport = aDest.AirportCode
    INNER JOIN Airline al ON fr.AirlineId = al.AirlineId
    INNER JOIN Aircraft ac ON fr.AircraftId = ac.AircraftId
    WHERE r.ReservationCode = @ReservationCode 
    AND EXISTS (
        SELECT 1 
        FROM Person p
        LEFT JOIN Buyer b ON p.PersonId = b.PersonId
        LEFT JOIN PassengerReservation pr ON p.PersonId = pr.PassengerId
        WHERE (b.BuyerId = r.BuyerId OR pr.ReservationId = r.ReservationId)
        AND (
            LOWER(p.FirstLastName) LIKE @LastNameSearch OR 
            LOWER(p.SecondLastName) LIKE @LastNameSearch OR 
            LOWER(CONCAT(p.FirstLastName, ' ', p.SecondLastName)) LIKE @LastNameSearch
        )
    )
    ORDER BY DepartureDateTime ASC;

    SELECT COUNT(DISTINCT PassengerId) 
    FROM BoardingPass 
    WHERE ReservationCode = @ReservationCode;
END