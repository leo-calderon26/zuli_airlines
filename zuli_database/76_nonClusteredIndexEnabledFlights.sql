USE ZuliAirlines;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.FlightRoute')
      AND name = N'IX_FlightRoute_Habilitada_Pagination'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_FlightRoute_Habilitada_Pagination
    ON dbo.FlightRoute (
        FlightRouteId
    )
    INCLUDE (
        Frequency,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        EstimatedDuration,
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice
    )
    WHERE Status = 'Habilitada';
END;
GO