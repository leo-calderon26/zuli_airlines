USE ZuliAirlines;
GO

ALTER TABLE dbo.FlightRoute DROP CONSTRAINT IF EXISTS FK_Airport_ArrivalAirport;
ALTER TABLE dbo.FlightRoute DROP CONSTRAINT IF EXISTS FK_Airport_DepartureAirport;
GO

ALTER TABLE dbo.FlightRoute 
ADD CONSTRAINT FK_Airport_ArrivalAirport
FOREIGN KEY (ArrivalAirport) REFERENCES dbo.Airport(AirportCode);

ALTER TABLE dbo.FlightRoute 
ADD CONSTRAINT FK_Airport_DepartureAirport
FOREIGN KEY (DepartureAirport) REFERENCES dbo.Airport(AirportCode);
GO