USE ZuliAirlines;
GO

IF COL_LENGTH('dbo.AirlineUser', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.AirlineUser
    ADD IsDeleted BIT NOT NULL
        CONSTRAINT DF_AirlineUser_IsDeleted
        DEFAULT (0) WITH VALUES;
END;
GO