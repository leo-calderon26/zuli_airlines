CREATE TYPE dbo.BaggageBulkType AS TABLE (
    PassengerId INT NOT NULL,
    ReservationId INT NOT NULL,
    Weight DECIMAL(6,2) NOT NULL,
    Size VARCHAR(10) NOT NULL,
    Type VARCHAR(20) NULL
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_BulkBaggage
    @Baggages dbo.BaggageBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Baggage (PassengerId, ReservationId, Weight, Size, Type)
    SELECT PassengerId, ReservationId, Weight, Size, Type FROM @Baggages;
END