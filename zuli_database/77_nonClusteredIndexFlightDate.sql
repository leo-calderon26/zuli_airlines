USE ZuliAirlines;
GO

CREATE NONCLUSTERED INDEX IX_Flight_FlightDate
ON dbo.Flight (FlightDate)
INCLUDE (RealDepartureAirport, RealArrivalAirport, FlightRouteId, FirstClassPrice, TouristPrice, CarryOnPrice, CheckedPrice);