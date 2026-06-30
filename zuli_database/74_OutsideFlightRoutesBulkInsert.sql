CREATE TYPE dbo.OutsideFlightRouteBulkType AS TABLE
(
    AirlineId INT NOT NULL,
    RealArrivalAirport VARCHAR(3) NULL,
    RealDepartureAirport VARCHAR(3) NULL,
    RealArrivalTime TIME NULL,
    RealDepartureTime TIME NULL,
    Frequency INT NOT NULL,
    Duration INT NOT NULL DEFAULT 0,
    CarryOnPrice DECIMAL(10, 2) NULL,
    CheckedPrice DECIMAL(10, 2) NULL,
    TouristPrice DECIMAL(10, 2) NOT NULL,
    FirstClassPrice DECIMAL(10, 2) NOT NULL
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_BulkOutsideFlightRoutes
    @OutsideFlightRoutes dbo.OutsideFlightRouteBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.FlightRoute(AdminId, AirlineId, ArrivalAirport, DepartureAirport, ScheduledArrivalTime, ScheduledDepartureTime, Frequency, EstimatedDuration, AircraftId, CarryOnPrice, CheckedPrice, TouristPrice, FirstClassPrice)
    SELECT '9820B62A-F878-45F1-B8F1-3AD5E1FC5C37' as AdminId, AirlineId, RealArrivalAirport, RealDepartureAirport, RealArrivalTime, RealDepartureTime, Frequency, Duration, '691981E1-FDA1-49D6-A1EE-EFD70A1993D2' as AircraftId, CarryOnPrice, CheckedPrice, TouristPrice, FirstClassPrice FROM @OutsideFlightRoutes as ofr
    WHERE NOT EXISTS (
        SELECT 1
            FROM FlightRoute
            WHERE arrivalAirport = ofr.RealArrivalAirport
            AND airlineId = ofr.AirlineId
            AND departureAirport = ofr.RealDepartureAirport
            AND frequency =ofr.Frequency
            AND scheduledArrivalTime = ofr.RealArrivalTime
            AND scheduledDepartureTime = ofr.RealDepartureTime
            AND estimatedDuration = ofr.Duration
            AND carryOnPrice = ofr.CarryOnPrice
            AND checkedPrice = ofr.CheckedPrice
            AND touristPrice = ofr.TouristPrice
            AND firstClassPrice = ofr.FirstClassPrice
    );

	UPDATE fr
    SET fr.Status = 'Habilitada'
    FROM FlightRoute fr
    INNER JOIN @OutsideFlightRoutes ofr
        ON fr.AirlineId = ofr.AirlineId
        AND fr.ArrivalAirport = ofr.RealArrivalAirport
        AND fr.DepartureAirport = ofr.RealDepartureAirport
        AND fr.ScheduledArrivalTime = ofr.RealArrivalTime
        AND fr.ScheduledDepartureTime = ofr.RealDepartureTime
        AND fr.Frequency = ofr.Frequency
        AND fr.EstimatedDuration = ofr.Duration
    WHERE fr.AirlineId <> 1 AND fr.Status = 'Deshabilitada';
END
GO