USE ZuliAirlines;
GO
CREATE OR ALTER PROCEDURE dbo.sp_UpsertBuyer
    @FirstName VARCHAR(50),
    @FirstLastName VARCHAR(50),
    @SecondLastName VARCHAR(50),
    @Email VARCHAR(254),
    @Phone VARCHAR(20)
AS
BEGIN
    DECLARE @ExistingBuyerId INT;
    SELECT @ExistingBuyerId = BuyerId FROM Buyer WHERE Email = @Email;
    IF @ExistingBuyerId IS NOT NULL
    BEGIN
        UPDATE Buyer
        SET FirstName = @FirstName,
            FirstLastName = @FirstLastName,
            SecondLastName = @SecondLastName,
            Phone = @Phone
        WHERE BuyerId = @ExistingBuyerId;
        SELECT @ExistingBuyerId AS BuyerId;
    END
    ELSE
    BEGIN
        INSERT INTO Buyer (FirstName, FirstLastName, SecondLastName, Email, Phone)
        VALUES (@FirstName, @FirstLastName, @SecondLastName, @Email, @Phone);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS BuyerId;
    END
END
GO
