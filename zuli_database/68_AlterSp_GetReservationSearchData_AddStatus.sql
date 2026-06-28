USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetReservationSearchData
    @ReservationCode NVARCHAR(50),
    @LastNameSearch NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        ISNULL(
            flight.RealDepartureTime,
            CAST(CAST(flight.FlightDate AS DATE) AS DATETIME) +
            CAST(flightRoute.ScheduledDepartureTime AS DATETIME)
        ) AS DepartureDateTime,

        ISNULL(
            flight.RealArrivalTime,
            CASE
                WHEN flightRoute.ScheduledArrivalTime <
                     flightRoute.ScheduledDepartureTime
                THEN DATEADD(
                    DAY,
                    1,
                    CAST(CAST(flight.FlightDate AS DATE) AS DATETIME) +
                    CAST(flightRoute.ScheduledArrivalTime AS DATETIME)
                )
                ELSE
                    CAST(CAST(flight.FlightDate AS DATE) AS DATETIME) +
                    CAST(flightRoute.ScheduledArrivalTime AS DATETIME)
            END
        ) AS ArrivalDateTime,

        originAirport.City AS OriginCity,
        originAirport.AirportCode AS OriginCode,
        destinationAirport.City AS DestinationCity,
        destinationAirport.AirportCode AS DestinationCode,
        reservation.FlightClass,
        airline.AirlineName AS Airline,
        CONCAT('ZL ', flightRoute.FlightRouteId) AS FlightNumber,
        aircraft.Model AS AircraftModel,
        reservation.ReservationStatusId
    FROM dbo.Reservation reservation
    INNER JOIN dbo.BoardingPass boardingPass
        ON reservation.ReservationCode = boardingPass.ReservationCode
    INNER JOIN dbo.Flight flight
        ON boardingPass.FlightId = flight.Id
    INNER JOIN dbo.FlightRoute flightRoute
        ON flight.FlightRouteId = flightRoute.FlightRouteId
    INNER JOIN dbo.Airport originAirport
        ON flightRoute.DepartureAirport = originAirport.AirportCode
    INNER JOIN dbo.Airport destinationAirport
        ON flightRoute.ArrivalAirport = destinationAirport.AirportCode
    INNER JOIN dbo.Airline airline
        ON flightRoute.AirlineId = airline.AirlineId
    INNER JOIN dbo.Aircraft aircraft
        ON flightRoute.AircraftId = aircraft.AircraftId
    WHERE reservation.ReservationCode = @ReservationCode
      AND EXISTS
      (
          SELECT 1
          FROM dbo.Person person
          LEFT JOIN dbo.Buyer buyer
              ON person.PersonId = buyer.PersonId
          LEFT JOIN dbo.PassengerReservation passengerReservation
              ON person.PersonId = passengerReservation.PassengerId
          WHERE
              (
                  buyer.BuyerId = reservation.BuyerId
                  OR passengerReservation.ReservationId = reservation.ReservationId
              )
              AND
              (
                  LOWER(person.FirstLastName) LIKE @LastNameSearch
                  OR LOWER(person.SecondLastName) LIKE @LastNameSearch
                  OR LOWER(
                      CONCAT(
                          person.FirstLastName,
                          ' ',
                          person.SecondLastName
                      )
                  ) LIKE @LastNameSearch
              )
      )
    ORDER BY DepartureDateTime ASC;

    SELECT COUNT(DISTINCT PassengerId) AS PassengerCount
    FROM dbo.BoardingPass
    WHERE ReservationCode = @ReservationCode;
END;
GO