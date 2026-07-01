USE ZuliAirlines;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Baggage')
      AND name = N'IX_Baggage_ReservationId_PassengerId'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Baggage_ReservationId_PassengerId
    ON dbo.Baggage (
        ReservationId,
        PassengerId
    )
    INCLUDE (
        Type,
        Weight
    );
END;
GO