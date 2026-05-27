USE ZuliAirlines;
GO

CREATE FUNCTION dbo.getAircraftTotalSeats (@AircraftId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @TotalSeats INT;
    SELECT @TotalSeats = ((NumberEconomyClassRows * NumberSeatingRowsEconomy) + (NumberFirstClassRows * NumberSeatingRowsFirst))
    FROM dbo.Aircraft
    WHERE AircraftId = @AircraftId;
    RETURN ISNULL(@TotalSeats, 0);
END;
GO
