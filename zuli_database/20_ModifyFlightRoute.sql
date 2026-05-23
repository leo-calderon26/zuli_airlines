USE ZuliAirlines;
GO
ALTER TABLE dbo.FlightRoute
ADD 
    CheckedBagMultiplier DECIMAL(5,2) NOT NULL DEFAULT 1.00,
    MaxWeightPerBag DECIMAL(6,2) NOT NULL DEFAULT 23.00;