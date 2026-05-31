USE ZuliAirlines;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Flight_RealDepartureAirport')
BEGIN
    ALTER TABLE dbo.Flight
    ADD CONSTRAINT FK_Flight_RealDepartureAirport
    FOREIGN KEY (RealDepartureAirport) REFERENCES dbo.Airport(AirportCode);
END
GO
