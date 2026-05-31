USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpsertBuyer
    @FirstName VARCHAR(50),
    @FirstLastName VARCHAR(50),
    @SecondLastName VARCHAR(50),
    @BirthDate VARCHAR(10),
    @Email VARCHAR(254),
    @Phone VARCHAR(20)
AS
BEGIN
    DECLARE @PersonId INT;

    SELECT @PersonId = PersonId
    FROM dbo.Person
    WHERE FirstName = @FirstName
      AND FirstLastName = @FirstLastName
      AND SecondLastName = @SecondLastName
      AND BirthDate = @BirthDate;

    IF @PersonId IS NULL
    BEGIN
        INSERT INTO dbo.Person (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
        VALUES (@FirstName, @FirstLastName, @SecondLastName, @BirthDate, '');

        SET @PersonId = SCOPE_IDENTITY();
    END

    IF @Email IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.PersonEmail WHERE PersonId = @PersonId)
            UPDATE dbo.PersonEmail SET Email = @Email WHERE PersonId = @PersonId;
        ELSE
            INSERT INTO dbo.PersonEmail (PersonId, Email) VALUES (@PersonId, @Email);
    END

    DECLARE @BuyerId INT;
    SELECT @BuyerId = BuyerId FROM dbo.Buyer WHERE PersonId = @PersonId;

    IF @BuyerId IS NOT NULL
    BEGIN
        UPDATE dbo.Buyer
        SET Phone = @Phone
        WHERE BuyerId = @BuyerId;

        SELECT @BuyerId AS BuyerId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.Buyer (PersonId, Phone)
        VALUES (@PersonId, @Phone);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS BuyerId;
    END
END
GO
