USE ZuliAirlines;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.FlightRoute') AND name = 'MaxWeightPerBag')
    ALTER TABLE dbo.FlightRoute ADD MaxWeightPerBag DECIMAL(6,2) NOT NULL DEFAULT 23.00;
GO

UPDATE dbo.FlightRoute
SET MaxWeightPerBag = 23.00
WHERE MaxWeightPerBag IS NULL OR MaxWeightPerBag = 0.00;
GO
