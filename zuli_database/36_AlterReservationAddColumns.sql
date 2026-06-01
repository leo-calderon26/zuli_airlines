USE ZuliAirlines;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservation') AND name = 'FlightClass')
    ALTER TABLE dbo.Reservation ADD FlightClass VARCHAR(20) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservation') AND name = 'PaymentMethod')
    ALTER TABLE dbo.Reservation ADD PaymentMethod VARCHAR(30) NULL;
GO
