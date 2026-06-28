USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RequestReservationCancellation
    @ReservationCode VARCHAR(20),
    @TokenHash VARCHAR(128),
    @RequestedAt DATETIME2,
    @ExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Reservation
    SET CancellationTokenHash = @TokenHash,
        CancellationRequestedAt = @RequestedAt,
        CancellationTokenExpiresAt = @ExpiresAt
    WHERE ReservationCode = @ReservationCode
      AND ReservationStatusId = 1;

    SELECT @@ROWCOUNT AS Result;
END;
GO