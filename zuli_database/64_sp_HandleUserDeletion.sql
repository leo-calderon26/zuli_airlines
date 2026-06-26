CREATE OR ALTER PROCEDURE dbo.sp_HandleUserDeletion
    @selectedUserToDelete UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    /*
        UserDeletionResult:
        1 = HardDeleted
        2 = SoftDeleted
        3 = Protected
        4 = NotFound
    */

    DECLARE @isProtected BIT;
    DECLARE @hasDependencies BIT = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @isProtected = au.IsProtected
        FROM dbo.AirlineUser AS au WITH (UPDLOCK, HOLDLOCK)
        WHERE au.UserId = @selectedUserToDelete
          AND au.IsDeleted = 0;

        IF @isProtected IS NULL
        BEGIN
            COMMIT TRANSACTION;

            SELECT CAST(4 AS INT) AS DeletionResult;
            RETURN;
        END;

        IF @isProtected = 1
        BEGIN
            COMMIT TRANSACTION;

            SELECT CAST(3 AS INT) AS DeletionResult;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.Aircraft WITH (UPDLOCK, HOLDLOCK)
            WHERE AdminId = @selectedUserToDelete
        )
        OR EXISTS
        (
            SELECT 1
            FROM dbo.Airport WITH (UPDLOCK, HOLDLOCK)
            WHERE AdminId = @selectedUserToDelete
        )
        OR EXISTS
        (
            SELECT 1
            FROM dbo.Flight WITH (UPDLOCK, HOLDLOCK)
            WHERE AdminId = @selectedUserToDelete
        )
        OR EXISTS
        (
            SELECT 1
            FROM dbo.FlightRoute WITH (UPDLOCK, HOLDLOCK)
            WHERE AdminId = @selectedUserToDelete
        )
        OR EXISTS
        (
            SELECT 1
            FROM dbo.AirlineUser WITH (UPDLOCK, HOLDLOCK)
            WHERE ManagedByAdminId = @selectedUserToDelete
        )
        BEGIN
            SET @hasDependencies = 1;
        END;

        IF @hasDependencies = 1
        BEGIN
            UPDATE dbo.AirlineUser
            SET
                IsActive = 0,
                IsDeleted = 1
            WHERE UserId = @selectedUserToDelete
              AND IsProtected = 0
              AND IsDeleted = 0;

            IF @@ROWCOUNT <> 1
            BEGIN
                THROW 50020, 'No se pudo realizar la eliminación lógica del usuario.', 1;
            END;

            COMMIT TRANSACTION;

            SELECT CAST(2 AS INT) AS DeletionResult;
            RETURN;
        END;

        DELETE FROM dbo.AirlineUser
        WHERE UserId = @selectedUserToDelete
          AND IsProtected = 0
          AND IsDeleted = 0;

        IF @@ROWCOUNT <> 1
        BEGIN
            THROW 50021, 'No se pudo realizar la eliminación física del usuario.', 1;
        END;

        COMMIT TRANSACTION;

        SELECT CAST(1 AS INT) AS DeletionResult;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO