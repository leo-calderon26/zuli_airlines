USE Zuliairlines;
GO

CREATE OR ALTER TRIGGER dbo.TR_BoardingPass_DecreaseAvailableSeats
ON dbo.BoardingPass
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InsertedSeats TABLE
    (
        FlightId UNIQUEIDENTIFIER PRIMARY KEY,
        SeatsToDiscount INT NOT NULL
    );

    INSERT INTO @InsertedSeats
    (
        FlightId,
        SeatsToDiscount
    )
    SELECT
        FlightId,
        COUNT(*) AS SeatsToDiscount
    FROM inserted
    GROUP BY FlightId;

    UPDATE f
    SET f.AvailableSeats = f.AvailableSeats - i.SeatsToDiscount
    FROM dbo.Flight f WITH (UPDLOCK, ROWLOCK)
    INNER JOIN @InsertedSeats i
        ON f.Id = i.FlightId
    WHERE f.AvailableSeats >= i.SeatsToDiscount;

    IF @@ROWCOUNT <> (
        SELECT COUNT(*)
        FROM @InsertedSeats
    )
    BEGIN
        THROW 50001, 'No hay suficientes asientos disponibles para completar la compra.', 1;
    END;
END;
GO