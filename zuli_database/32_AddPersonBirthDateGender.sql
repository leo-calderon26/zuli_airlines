USE ZuliAirlines;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Person') AND name = 'BirthDate')
    ALTER TABLE dbo.Person ADD BirthDate VARCHAR(10) NOT NULL DEFAULT '';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Person') AND name = 'Gender')
    ALTER TABLE dbo.Person ADD Gender VARCHAR(20) NOT NULL DEFAULT '';
GO
