USE ZuliAirlines;
GO

EXEC sp_rename 'dbo.FlightRoute.ScheduledDeparture', 'ScheduledDepartureTime', 'COLUMN';
GO

ALTER TABLE dbo.FlightRoute
ALTER COLUMN ScheduledArrivalTime TIME NOT NULL;

ALTER TABLE dbo.FlightRoute
ALTER COLUMN ScheduledDepartureTime TIME NOT NULL;
GO

ALTER TABLE dbo.FlightRoute
ADD 
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    CarryOnPrice DECIMAL(10, 2) NULL,
    CheckedPrice DECIMAL(10, 2) NULL,
    TouristPrice DECIMAL(10, 2) NULL,
    FirstClassPrice DECIMAL(10, 2) NULL;
GO

ALTER TABLE dbo.FlightRoute
ADD CONSTRAINT FK_FlightRoute_Aircraft
FOREIGN KEY (AircraftId) 
REFERENCES dbo.Aircraft(AircraftId);
GO

ALTER TABLE dbo.FlightRoute
ADD CONSTRAINT CK_FlightRoute_TouristPrice 
CHECK (TouristPrice >= 0);

ALTER TABLE dbo.FlightRoute
ADD CONSTRAINT CK_FlightRoute_FirstClassPrice 
CHECK (FirstClassPrice >= 0);

ALTER TABLE dbo.Flight 
ADD CONSTRAINT CK_Flight_CarryOnPrice 
CHECK (CarryOnPrice >= 0);

ALTER TABLE dbo.Flight 
ADD CONSTRAINT CK_Flight_CheckedPrice 
CHECK (CheckedPrice >= 0);
GO