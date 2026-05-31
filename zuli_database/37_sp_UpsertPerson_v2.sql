USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpsertPerson
    @FirstName VARCHAR(50),
    @FirstLastName VARCHAR(50),
    @SecondLastName VARCHAR(50),
    @BirthDate VARCHAR(10),
    @Gender VARCHAR(10),
    @Email VARCHAR(254) = NULL
AS
BEGIN
    SET NOCOUNT ON;
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
        VALUES (@FirstName, @FirstLastName, @SecondLastName, @BirthDate, @Gender);

        SET @PersonId = SCOPE_IDENTITY();
    END

    IF @Email IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.PersonEmail WHERE PersonId = @PersonId)
            UPDATE dbo.PersonEmail SET Email = @Email WHERE PersonId = @PersonId;
        ELSE
            INSERT INTO dbo.PersonEmail (PersonId, Email) VALUES (@PersonId, @Email);
    END

    SELECT @PersonId AS PersonId;
END
GO
