CREATE TYPE dbo.OutsideFlightRouteBulkType AS TABLE
(
    AirlineId INT NOT NULL,
    ArrivalAirportCode VARCHAR(3) NULL,
    ArrivalAirportName VARCHAR(100) NULL,
    ArrivalAirportCity VARCHAR(60) NULL,
    DepartureAirportCode VARCHAR(3) NULL,
    DepartureAirportName VARCHAR(100) NULL,
    DepartureAirportCity VARCHAR(60) NULL,
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
    BEGIN TRAN
        SET NOCOUNT ON;
        declare @adminId uniqueidentifier;
        set @adminId = (SELECT UserId from AirlineUser WHERE BusinessID = '100000000');
        declare @aircraftId uniqueidentifier;
        set @aircraftId = (SELECT AircraftId from Aircraft WHERE Model = 'Yigui-Externo');
        WITH Airports AS (
            SELECT DISTINCT ArrivalAirportCode AS AirportCode,
                    ArrivalAirportName AS Name,
                    ArrivalAirportCity AS City
            FROM @OutsideFlightRoutes

            UNION

            SELECT DISTINCT DepartureAirportCode,
                    DepartureAirportName,
                    DepartureAirportCity
            FROM @OutsideFlightRoutes
        )
        INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
        SELECT a.AirportCode, a.Name, a.City, a.City, @adminId
        FROM Airports a
        WHERE NOT EXISTS (
            SELECT 1
            FROM dbo.Airport ap
            WHERE ap.AirportCode = a.AirportCode
        );

        INSERT INTO dbo.FlightRoute(AdminId, AirlineId, ArrivalAirport, DepartureAirport, ScheduledArrivalTime, ScheduledDepartureTime, Frequency, EstimatedDuration, AircraftId, CarryOnPrice, CheckedPrice, TouristPrice, FirstClassPrice)
        SELECT @adminId as AdminId, AirlineId, ArrivalAirportCode as ArrivalAirport, DepartureAirportCode as DepartureAirport, RealArrivalTime, RealDepartureTime, Frequency, Duration, @aircraftId as AircraftId, CarryOnPrice, CheckedPrice, TouristPrice, FirstClassPrice FROM @OutsideFlightRoutes as ofr
        WHERE NOT EXISTS (
            SELECT 1
                FROM FlightRoute
                WHERE arrivalAirport = ofr.ArrivalAirportCode
                AND airlineId = ofr.AirlineId
                AND departureAirport = ofr.DepartureAirportCode
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
            AND fr.ArrivalAirport = ofr.ArrivalAirportCode
            AND fr.DepartureAirport = ofr.DepartureAirportCode
            AND fr.ScheduledArrivalTime = ofr.RealArrivalTime
            AND fr.ScheduledDepartureTime = ofr.RealDepartureTime
            AND fr.Frequency = ofr.Frequency
            AND fr.EstimatedDuration = ofr.Duration
        WHERE fr.AirlineId <> 1 AND fr.Status = 'Deshabilitada';
    COMMIT
END
GO