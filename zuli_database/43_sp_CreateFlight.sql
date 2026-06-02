CREATE PROCEDURE dbo.sp_CreateFlight
    @FlightRouteId INT,
    @DepartureDate DATE
AS
BEGIN
    DECLARE @FlightId UNIQUEIDENTIFIER;
    SET @FlightId = newId();
    BEGIN
        INSERT INTO dbo.Flight (Id, Status, FlightDate, TouristPrice, FirstClassPrice, RealDepartureTime, RealArrivalTime, AircraftId, Duration, CarryOnPrice, CheckedPrice,
        AvailableSeats, AdminId, FlightRouteId, RealArrivalAirport, RealDepartureAirport)
        SELECT @FlightId as Id, 'Programado' as Status, @DepartureDate as FlightDate, fr.TouristPrice, fr.FirstClassPrice, (CAST(@DepartureDate AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME)) as RealDepartureTime, (CAST(@DepartureDate AS DATETIME) + CAST(fr.ScheduledArrivalTime AS DATETIME)) as RealArrivalTime, fr.AircraftId, fr.EstimatedDuration, fr.CarryOnPrice,
        fr.CheckedPrice, dbo.getAircraftTotalSeats(a.AircraftId) as AvailableSeats, fr.AdminId, fr.FlightRouteId, fr.ArrivalAirport, fr.DepartureAirport from FlightRoute fr INNER JOIN Aircraft a on fr.AircraftId = a.AircraftId where FlightRouteId = @FlightRouteId
    END
    SELECT @FlightId as FlightId
END
GO