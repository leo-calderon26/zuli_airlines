USE ZuliAirlines;
GO

ALTER TABLE dbo.Flight
ADD Duration INT NOT NULL DEFAULT 0;

GO

ALTER TABLE dbo.Flight
ADD CONSTRAINT CK_Flight_Duration
CHECK (Duration >= 0);
