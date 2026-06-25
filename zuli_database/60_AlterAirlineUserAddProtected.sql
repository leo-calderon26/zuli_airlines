USE ZuliAirlines;
GO

IF COL_LENGTH('dbo.AirlineUser', 'IsProtected') IS NULL
BEGIN
    ALTER TABLE dbo.AirlineUser
    ADD IsProtected BIT NOT NULL
        CONSTRAINT DF_AirlineUser_IsProtected
        DEFAULT (0) WITH VALUES;
END;
GO