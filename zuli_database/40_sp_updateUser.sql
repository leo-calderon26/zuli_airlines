USE ZuliAirlines;
GO

CREATE OR ALTER PROCEDURE dbo.sp_updateUser
    @UserId UNIQUEIDENTIFIER,
    @PersonId INT,
    @FirstName VARCHAR(50),
    @FirstLastName VARCHAR(50),
    @SecondLastName VARCHAR(50),
    @BusinessEmail VARCHAR(254),
    @UserRole VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Person
    SET
        FirstName = @FirstName,
        FirstLastName = @FirstLastName,
        SecondLastName = @SecondLastName
    WHERE PersonId = @PersonId;

    UPDATE dbo.AirlineUser
    SET
        BusinessEmail = @BusinessEmail,
        UserRole = @UserRole
    WHERE UserId = @UserId;
END
GO