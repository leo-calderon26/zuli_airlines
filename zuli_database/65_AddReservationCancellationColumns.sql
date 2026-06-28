USE ZuliAirlines;
GO

SET NOCOUNT ON;
GO

IF OBJECT_ID(N'dbo.Reservation', N'U') IS NULL
BEGIN
    THROW 50001, 'The dbo.Reservation table does not exist.', 1;
END;
GO

/*   Reservation status code
   1 = Active
   2 = Cancelled 
*/

IF COL_LENGTH(N'dbo.Reservation', N'ReservationStatusId') IS NULL
BEGIN
    EXEC sys.sp_executesql N'
        ALTER TABLE dbo.Reservation
        ADD ReservationStatusId TINYINT NOT NULL
            CONSTRAINT DF_Reservation_ReservationStatusId
            DEFAULT (1)
            WITH VALUES;
    ';
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.Reservation')
      AND name = N'CK_Reservation_ReservationStatusId'
)
BEGIN
    EXEC sys.sp_executesql N'
        ALTER TABLE dbo.Reservation
        ADD CONSTRAINT CK_Reservation_ReservationStatusId
        CHECK (ReservationStatusId IN (1, 2));
    ';
END;
GO


IF COL_LENGTH(N'dbo.Reservation', N'CancellationTokenHash') IS NULL
BEGIN
    EXEC sys.sp_executesql N'
        ALTER TABLE dbo.Reservation
        ADD CancellationTokenHash VARCHAR(128) NULL;
    ';
END;
GO

IF COL_LENGTH(N'dbo.Reservation', N'CancellationRequestedAt') IS NULL
BEGIN
    EXEC sys.sp_executesql N'
        ALTER TABLE dbo.Reservation
        ADD CancellationRequestedAt DATETIME2 NULL;
    ';
END;
GO

IF COL_LENGTH(N'dbo.Reservation', N'CancellationTokenExpiresAt') IS NULL
BEGIN
    EXEC sys.sp_executesql N'
        ALTER TABLE dbo.Reservation
        ADD CancellationTokenExpiresAt DATETIME2 NULL;
    ';
END;
GO

IF COL_LENGTH(N'dbo.Reservation', N'CancelledAt') IS NULL
BEGIN
    EXEC sys.sp_executesql N'
        ALTER TABLE dbo.Reservation
        ADD CancelledAt DATETIME2 NULL;
    ';
END;
GO
