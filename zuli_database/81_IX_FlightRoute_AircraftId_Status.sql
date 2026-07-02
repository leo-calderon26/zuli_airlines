USE zuli;
GO
CREATE NONCLUSTERED INDEX IX_FlightRoute_AircraftId_Status
ON dbo.FlightRoute (AircraftId,Status);
GO