USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_HandleAirportDeletion
    @selectedAirportToDelete CHAR(3)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    DECLARE @airportSoftDelete BIT;
    DECLARE @currentFlightRoute INT;

    SET @airportSoftDelete = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- CURSOR 1 Detectar rutas con vuelos agendados para hacer soft delete
        DECLARE airport_softDelete_cursor CURSOR FOR 
        SELECT DISTINCT fr.FlightRouteId
    FROM dbo.Airport ac
        INNER JOIN dbo.FlightRoute fr ON ac.AirportCode = fr.ArrivalAirport OR ac.AirportCode = fr.DepartureAirport
        INNER JOIN dbo.Flight f ON fr.FlightRouteId = f.FlightRouteId
    WHERE ac.AirportCode = @selectedAirportToDelete;
     
        OPEN airport_softDelete_cursor;
        FETCH NEXT FROM airport_softDelete_cursor INTO @currentFlightRoute;  
     
        WHILE @@FETCH_STATUS = 0  
        BEGIN
        SET @airportSoftDelete = 1;
        UPDATE dbo.FlightRoute SET Status = 'Deshabilitada' WHERE FlightRouteId = @currentFlightRoute;

        FETCH NEXT FROM airport_softDelete_cursor INTO @currentFlightRoute;
    END; 
     
        CLOSE airport_softDelete_cursor;
        DEALLOCATE airport_softDelete_cursor;

        -- Marcar el aeropuerto como eliminado (soft delete) si se encontraron rutas con vuelos agendados
        UPDATE dbo.Airport 
        SET IsDeleted = @airportSoftDelete 
        WHERE AirportCode = @selectedAirportToDelete;

        -- CURSOR 2 Limpieza Manual (Sustituye al CASCADE que falló en SQL Server por caminos múltiples)
        DECLARE airport_hardDelete_cursor CURSOR FOR 
        SELECT fr.FlightRouteId
    FROM dbo.Airport ac
        INNER JOIN dbo.FlightRoute fr ON ac.AirportCode = fr.ArrivalAirport OR ac.AirportCode = fr.DepartureAirport
    WHERE ac.AirportCode = @selectedAirportToDelete
        AND fr.FlightRouteId NOT IN (SELECT FlightRouteId
        FROM dbo.Flight);

        OPEN airport_hardDelete_cursor;
        FETCH NEXT FROM airport_hardDelete_cursor INTO @currentFlightRoute;  

        WHILE @@FETCH_STATUS = 0
        BEGIN
        -- Borramos primero la ruta hija manualmente porque CASCADE fallaba
        DELETE FROM dbo.FlightRoute WHERE FlightRouteId = @currentFlightRoute;

        FETCH NEXT FROM airport_hardDelete_cursor INTO @currentFlightRoute;
    END; 
     
        CLOSE airport_hardDelete_cursor;
        DEALLOCATE airport_hardDelete_cursor;

        -- Eliminar el aeropuerto si no tiene rutas asociadas con vuelos (hard delete)
        DELETE FROM dbo.Airport
        WHERE AirportCode = @selectedAirportToDelete AND IsDeleted = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
        BEGIN
        ROLLBACK TRANSACTION;
    END;

        IF CURSOR_STATUS('global', 'airport_softDelete_cursor') >= 0
        BEGIN
        CLOSE airport_softDelete_cursor;
        DEALLOCATE airport_softDelete_cursor;
    END;
        IF CURSOR_STATUS('global', 'airport_hardDelete_cursor') >= 0
        BEGIN
        CLOSE airport_hardDelete_cursor;
        DEALLOCATE airport_hardDelete_cursor;
    END;

        THROW;
    END CATCH;
END;
GO