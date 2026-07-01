USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_HandleFlightRouteDeletion
    @selectedFlightRouteToDelete INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @routeIsActive BIT = 0;
    DECLARE @hasHistoricalRecords BIT = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        /*
            A route can be deleted only while it is enabled.

            Result codes:
            1 = HardDeleted
            2 = SoftDeleted
            4 = NotFoundOrAlreadyDisabled
        */
        IF EXISTS
        (
            SELECT 1
            FROM dbo.FlightRoute WITH (UPDLOCK, HOLDLOCK)
            WHERE FlightRouteId = @selectedFlightRouteToDelete
              AND Status = 'Habilitada'
        )
        BEGIN
            SET @routeIsActive = 1;
        END;

        IF @routeIsActive = 0
        BEGIN
            COMMIT TRANSACTION;

            SELECT CAST(4 AS INT) AS DeletionResult;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.Flight flight
            INNER JOIN dbo.BoardingPass boardingPass
                ON boardingPass.FlightId = flight.Id
            WHERE flight.FlightRouteId = @selectedFlightRouteToDelete
        )
        BEGIN
            SET @hasHistoricalRecords = 1;
        END;

        IF @hasHistoricalRecords = 1
        BEGIN
            UPDATE dbo.FlightRoute
            SET Status = 'Deshabilitada'
            WHERE FlightRouteId = @selectedFlightRouteToDelete
              AND Status = 'Habilitada';

            IF @@ROWCOUNT <> 1
            BEGIN
                THROW 50030,
                    'The flight route could not be disabled.',
                    1;
            END;

            COMMIT TRANSACTION;

            SELECT CAST(2 AS INT) AS DeletionResult;
            RETURN;
        END;


        DELETE FROM dbo.Flight
        WHERE FlightRouteId = @selectedFlightRouteToDelete;

        DELETE FROM dbo.FlightRoute
        WHERE FlightRouteId = @selectedFlightRouteToDelete
          AND Status = 'Habilitada';

        IF @@ROWCOUNT <> 1
        BEGIN
            THROW 50031,
                'The flight route could not be deleted.',
                1;
        END;

        COMMIT TRANSACTION;

        SELECT CAST(1 AS INT) AS DeletionResult;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO