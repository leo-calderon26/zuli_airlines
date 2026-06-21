USE ZuliAirlines;
GO

CREATE OR ALTER FUNCTION dbo.GetIncomeReport (
    @Year        INT,
    @Origin      VARCHAR(3) = NULL,
    @Destination VARCHAR(3) = NULL,
    @AirlineId   INT = NULL
)
RETURNS TABLE
AS
RETURN
(
    WITH Months AS (
        SELECT 1 AS MonthNumber UNION ALL SELECT 2 UNION ALL SELECT 3
        UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6
        UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9
        UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
    ),

    Combination AS (
        SELECT
            @Year AS YearNumber,
            m.MonthNumber
        FROM Months m
    ),

    qualifying_flights AS (
        SELECT
            f.Id AS FlightId,
            f.FlightDate,
            f.TouristPrice,
            f.FirstClassPrice,
            f.CarryOnPrice,
            f.CheckedPrice,
            fr.CheckedBagMultiplier
        FROM dbo.Flight f
        INNER JOIN dbo.FlightRoute fr
            ON f.FlightRouteId = fr.FlightRouteId
        WHERE f.FlightDate >= DATEFROMPARTS(@Year, 1, 1)
          AND f.FlightDate <  DATEFROMPARTS(@Year + 1, 1, 1)
          AND (@AirlineId IS NULL OR fr.AirlineId = @AirlineId)
          AND (@Origin IS NULL OR fr.DepartureAirport = @Origin)
          AND (@Destination IS NULL OR fr.ArrivalAirport = @Destination)
    ),

    boarding AS (
        SELECT
            qf.FlightId,
            YEAR(qf.FlightDate) AS FlightYear,
            MONTH(qf.FlightDate) AS FlightMonth,
            qf.TouristPrice,
            qf.FirstClassPrice,
            bp.ReservationCode,
            bp.PassengerId,
            r.ReservationId,
            CASE
                WHEN LOWER(ISNULL(r.FlightClass, '')) = 'primera clase'
                THEN 1 ELSE 0
            END AS IsFirstClass
        FROM qualifying_flights qf
        INNER JOIN dbo.BoardingPass bp
            ON bp.FlightId = qf.FlightId
        INNER JOIN dbo.Reservation r
            ON r.ReservationCode = bp.ReservationCode
    ),

    checked_bags AS (
        SELECT 
            b.PassengerId,
            b.ReservationId,
            ROW_NUMBER() OVER (
                PARTITION BY b.PassengerId, b.ReservationId
                ORDER BY b.BaggageId
            ) AS BagNumber
        FROM dbo.Baggage b
        WHERE LOWER(ISNULL(b.Type, '')) = 'checked'
    ),

    checked_bags_income AS (
        SELECT
            bp.FlightId,
            bp.ReservationCode,
            bp.PassengerId,
            SUM(
                qf.CheckedPrice *
                POWER(qf.CheckedBagMultiplier, cb.BagNumber - 1)
            ) AS checked_income
        FROM boarding bp
        INNER JOIN checked_bags cb
            ON cb.PassengerId = bp.PassengerId
           AND cb.ReservationId = bp.ReservationId
        INNER JOIN qualifying_flights qf
            ON qf.FlightId = bp.FlightId
        GROUP BY
            bp.FlightId,
            bp.ReservationCode,
            bp.PassengerId
    ),

    carryon_counts AS (
        SELECT
            b.PassengerId,
            b.ReservationId,
            COUNT(*) AS qty
        FROM dbo.Baggage b
        WHERE LOWER(ISNULL(b.Type,'')) = 'mano'
        GROUP BY b.PassengerId, b.ReservationId
    ),

    carryon_income AS (
        SELECT
            bp.FlightId,
            bp.ReservationCode,
            bp.PassengerId,
            co.qty * qf.CarryOnPrice AS carryon_income
        FROM boarding bp
        INNER JOIN carryon_counts co
            ON co.PassengerId = bp.PassengerId
           AND co.ReservationId = bp.ReservationId
        INNER JOIN qualifying_flights qf
            ON qf.FlightId = bp.FlightId
    ),

    baggage_per_bp AS (
        SELECT
            bp.FlightId,
            bp.ReservationCode,
            bp.PassengerId,
            ISNULL(ci.carryon_income, 0)
            + ISNULL(cbi.checked_income, 0) AS baggage_income
        FROM boarding bp
        LEFT JOIN checked_bags_income cbi
            ON cbi.FlightId = bp.FlightId
           AND cbi.ReservationCode = bp.ReservationCode
           AND cbi.PassengerId = bp.PassengerId
        LEFT JOIN carryon_income ci
            ON ci.FlightId = bp.FlightId
           AND ci.ReservationCode = bp.ReservationCode
           AND ci.PassengerId = bp.PassengerId
    )

    SELECT
        c.YearNumber AS [Year],
        c.MonthNumber AS [Month],

        ISNULL(COUNT(DISTINCT qf.FlightId), 0) AS Flights,
        ISNULL(SUM(bp.IsFirstClass), 0) AS FirstClass,
        ISNULL(SUM(CASE WHEN bp.IsFirstClass = 0 THEN 1 ELSE 0 END), 0) AS TouristClass,
        ISNULL(COUNT(bp.PassengerId), 0) AS TotalPassengers,

        ISNULL(SUM(
            CASE WHEN bp.IsFirstClass = 1
                THEN bp.FirstClassPrice
                ELSE bp.TouristPrice
            END
        ), 0) AS TicketIncome,

        ISNULL(SUM(bbi.baggage_income), 0) AS BaggageIncome,

        ISNULL(SUM(
            CASE WHEN bp.IsFirstClass = 1
                THEN bp.FirstClassPrice
                ELSE bp.TouristPrice
            END
        ), 0)
        + ISNULL(SUM(bbi.baggage_income), 0) AS TotalIncome

    FROM Combination c

    LEFT JOIN qualifying_flights qf
        ON YEAR(qf.FlightDate) = c.YearNumber
       AND MONTH(qf.FlightDate) = c.MonthNumber

    LEFT JOIN boarding bp
        ON bp.FlightId = qf.FlightId

    LEFT JOIN baggage_per_bp bbi
        ON bbi.FlightId = bp.FlightId
       AND bbi.ReservationCode = bp.ReservationCode
       AND bbi.PassengerId = bp.PassengerId

    GROUP BY
        c.YearNumber,
        c.MonthNumber
);
GO