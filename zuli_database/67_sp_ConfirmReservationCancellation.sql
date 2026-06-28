USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ConfirmReservationCancellation
    @TokenHash VARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ReservationId INT;
    DECLARE @ReservationCode VARCHAR(20);
    DECLARE @CurrentUtcDateTime DATETIME2 = SYSUTCDATETIME();

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @ReservationId = reservation.ReservationId,
            @ReservationCode = reservation.ReservationCode
        FROM dbo.Reservation reservation WITH (UPDLOCK, HOLDLOCK)
        WHERE reservation.CancellationTokenHash = @TokenHash
          AND reservation.ReservationStatusId = 1
          AND reservation.CancellationTokenExpiresAt > @CurrentUtcDateTime;

        IF @ReservationId IS NULL
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT CAST(0 AS INT) AS Result;
            RETURN;
        END;

        UPDATE flight
        SET flight.AvailableSeats =
            flight.AvailableSeats + boardingPassCount.SeatsToRestore
        FROM dbo.Flight flight
        INNER JOIN
        (
            SELECT
                boardingPass.FlightId,
                COUNT(*) AS SeatsToRestore
            FROM dbo.BoardingPass boardingPass
            WHERE boardingPass.ReservationCode = @ReservationCode
            GROUP BY boardingPass.FlightId
        ) boardingPassCount
            ON boardingPassCount.FlightId = flight.Id;

        DELETE FROM dbo.Baggage
        WHERE ReservationId = @ReservationId;

        UPDATE dbo.Reservation
        SET ReservationStatusId = 2,
            CancelledAt = @CurrentUtcDateTime,
            CancellationTokenHash = NULL,
            CancellationTokenExpiresAt = NULL
        WHERE ReservationId = @ReservationId
          AND ReservationStatusId = 1;

        COMMIT TRANSACTION;

        SELECT CAST(1 AS INT) AS Result;
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