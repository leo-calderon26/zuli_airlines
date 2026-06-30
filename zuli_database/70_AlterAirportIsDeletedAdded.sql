USE ZuliAirlines;
GO

IF COL_LENGTH('dbo.Airport', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Airport
    ADD IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Airport_IsDeleted DEFAULT (0) WITH VALUES;
END;
GO