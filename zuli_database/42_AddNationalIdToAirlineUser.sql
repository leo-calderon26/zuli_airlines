USE ZuliAirlines;
GO

ALTER TABLE dbo.AirlineUser
ADD NationalId CHAR(9) NULL;
GO

UPDATE dbo.AirlineUser 
SET NationalId = '000000000' 
WHERE NationalId IS NULL;
GO

ALTER TABLE dbo.AirlineUser
ALTER COLUMN NationalId CHAR(9) NOT NULL;
GO

ALTER TABLE dbo.AirlineUser
ADD CONSTRAINT UQ_AirlineUser_NationalId UNIQUE (NationalId);
GO
