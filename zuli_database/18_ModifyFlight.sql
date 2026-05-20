USE ZuliAirlines;
GO

ALTER TABLE dbo.Flight
DROP CONSTRAINT FK_Flight_Airline;

ALTER TABLE dbo.Flight
DROP COLUMN AirlineId;
GO

ALTER TABLE dbo.Flight
DROP COLUMN CheckInStartTime;

ALTER TABLE dbo.Flight
DROP COLUMN CheckInDeadline;
GO

ALTER TABLE dbo.Flight
ADD 
    RealArrivalAirport VARCHAR(3) NULL,
    RealDepartureAirport VARCHAR(3) NULL;
GO

ALTER TABLE dbo.Flight
ADD CONSTRAINT FK_Flight_RealArrivalAirport
FOREIGN KEY (RealArrivalAirport) 
REFERENCES dbo.Airport(AirportCode);

ALTER TABLE dbo.Flight
ADD CONSTRAINT FK_Flight_RealDepartureAirport
FOREIGN KEY (RealDepartureAirport) 
REFERENCES dbo.Airport(AirportCode);
GO
