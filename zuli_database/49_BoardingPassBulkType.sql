CREATE TYPE dbo.BoardingPassBulkType AS TABLE (
    FlightId UNIQUEIDENTIFIER NOT NULL,
    ReservationCode VARCHAR(20) NOT NULL,
    PassengerId INT NOT NULL
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_BulkBoardingPass
    @BoardingPasses dbo.BoardingPassBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.BoardingPass (FlightId, ReservationCode, PassengerId)
    SELECT FlightId, ReservationCode, PassengerId FROM @BoardingPasses;
END
GO