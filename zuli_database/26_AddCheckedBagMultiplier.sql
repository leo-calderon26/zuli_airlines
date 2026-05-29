ALTER TABLE dbo.FlightRoute
ADD CheckedBagMultiplier DECIMAL(10, 2) NOT NULL DEFAULT 1.0;
GO

UPDATE dbo.FlightRoute
SET CheckedBagMultiplier = 1.0
WHERE CheckedBagMultiplier IS NULL;
GO