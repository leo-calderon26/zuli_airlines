CREATE TYPE dbo.PassengerReservationBulkType AS TABLE (
    PassengerId INT NOT NULL,
    ReservationId INT NOT NULL
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_BulkPassengerReservation
    @PassengerReservations dbo.PassengerReservationBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.PassengerReservation (PassengerId, ReservationId)
    SELECT PassengerId, ReservationId FROM @PassengerReservations;
END