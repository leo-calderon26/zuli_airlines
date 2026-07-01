USE ZuliAirlines;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.FlightRoute')
      AND name = 'IX_FlightRoute_AircraftId_Status'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_FlightRoute_AircraftId_Status
    ON dbo.FlightRoute (
        AircraftId,
        Status
    );
END;
GO