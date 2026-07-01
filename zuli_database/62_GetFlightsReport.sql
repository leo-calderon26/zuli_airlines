CREATE OR ALTER FUNCTION dbo.GetFlightsReport
(
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @Origin VARCHAR(10) = NULL,
    @Destination VARCHAR(10) = NULL,
    @FlightClass VARCHAR(50) = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT
    f.FlightDate AS Fecha,
    f.RealDepartureAirport AS Origin,
    f.RealArrivalAirport AS Destination,
    f.FlightRouteId AS FlightNumber,
    a.AirlineName AS Aerolinea,

    SUM(CASE WHEN r.FlightClass = 'Primera Clase' THEN 1 ELSE 0 END) AS FirstClassPassengers,
    SUM(CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END) AS EconomyPassengers,

    (SUM(CASE WHEN r.FlightClass = 'Primera Clase' THEN 1 ELSE 0 END) * f.FirstClassPrice) +
        (SUM(CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END) * f.TouristPrice) AS PassengerSales,

    SUM(CASE WHEN bp.FlightId IS NOT NULL THEN (ISNULL(f.CarryOnPrice, 0.00) + ISNULL(f.CheckedPrice, 0.00)) ELSE 0.00 END) AS BaggageSales,

    ((SUM(CASE WHEN r.FlightClass = 'Primera Clase' THEN 1 ELSE 0 END) * f.FirstClassPrice) +
         SUM(CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END) * f.TouristPrice) +
        SUM(CASE WHEN bp.FlightId IS NOT NULL THEN (ISNULL(f.CarryOnPrice, 0.00) + ISNULL(f.CheckedPrice, 0.00)) ELSE 0.00 END) AS TotalSales

FROM dbo.Flight f
    INNER JOIN dbo.FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
    INNER JOIN dbo.Airline a ON fr.AirlineId = a.AirlineId
    LEFT JOIN dbo.BoardingPass bp ON f.Id = bp.FlightId
    LEFT JOIN dbo.Reservation r ON bp.reservationCode = r.reservationCode

WHERE 
        (@FromDate IS NULL OR f.FlightDate >= @FromDate)
    AND (@ToDate IS NULL OR f.FlightDate <= @ToDate)
    AND (@Origin IS NULL OR @Origin = '' OR f.RealDepartureAirport = @Origin)
    AND (@Destination IS NULL OR @Destination = '' OR f.RealArrivalAirport = @Destination)
    AND (@FlightClass IS NULL OR @FlightClass = '' OR r.FlightClass = @FlightClass)

GROUP BY 
        f.Id, 
        f.FlightDate,
        f.RealDepartureAirport,
        f.RealArrivalAirport,
        f.FlightRouteId,
        a.AirlineName,
        f.FirstClassPrice,  
        f.TouristPrice
);