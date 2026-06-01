USE ZuliAirlines;
GO
ALTER TABLE dbo.AirlineUser
ADD NationalId CHAR(9) NOT NULL UNIQUE;
GO
