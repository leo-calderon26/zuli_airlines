CREATE OR ALTER PROCEDURE dbo.sp_UpsertPersonBulk
    @Persons dbo.PersonBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Insertar/actualizar personas
    MERGE INTO dbo.Person AS Target
    USING (
        SELECT DISTINCT FirstName, FirstLastName, SecondLastName, BirthDate, Gender
        FROM @Persons
    ) AS Source
    ON Target.FirstName = Source.FirstName
        AND Target.FirstLastName = Source.FirstLastName
        AND Target.SecondLastName = Source.SecondLastName
        AND Target.BirthDate = Source.BirthDate
        AND Target.Gender = Source.Gender
    WHEN NOT MATCHED THEN
        INSERT (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
        VALUES (Source.FirstName, Source.FirstLastName, Source.SecondLastName, Source.BirthDate, Source.Gender);

    -- 2. Insertar/actualizar correos electrónicos
    MERGE INTO dbo.PersonEmail AS Target
    USING (
        SELECT p.PersonId, Source.Email
        FROM @Persons Source
        INNER JOIN dbo.Person p
            ON p.FirstName = Source.FirstName
            AND p.FirstLastName = Source.FirstLastName
            AND p.SecondLastName = Source.SecondLastName
            AND p.BirthDate = Source.BirthDate
            AND p.Gender = Source.Gender
        WHERE Source.Email IS NOT NULL
    ) AS Source
    ON Target.PersonId = Source.PersonId
    WHEN NOT MATCHED THEN
        INSERT (PersonId, Email)
        VALUES (Source.PersonId, Source.Email)
    WHEN MATCHED THEN
        UPDATE SET Email = Source.Email;

    -- 3. Insertar pasaportes (solo para pasajeros, no buyers)
    INSERT INTO dbo.Passport (PassengerId, PassportCountry, DueDate)
    SELECT p.PersonId, Source.PassportCountry, Source.PassportDueDate
    FROM @Persons Source
    INNER JOIN dbo.Person p
        ON p.FirstName = Source.FirstName
        AND p.FirstLastName = Source.FirstLastName
        AND p.SecondLastName = Source.SecondLastName
        AND p.BirthDate = Source.BirthDate
    WHERE Source.IsBuyer = 0
      AND Source.PassportCountry IS NOT NULL
      AND Source.PassportCountry <> ''
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Passport pas
          WHERE pas.PassengerId = p.PersonId
          AND pas.PassportCountry = Source.PassportCountry
      );

    -- 4. Insertar/actualizar Buyers (IsBuyer = 1)
    MERGE INTO dbo.Buyer AS Target
    USING (
        SELECT p.PersonId, Source.Phone
        FROM @Persons Source
        INNER JOIN dbo.Person p
            ON p.FirstName = Source.FirstName
            AND p.FirstLastName = Source.FirstLastName
            AND p.SecondLastName = Source.SecondLastName
            AND p.BirthDate = Source.BirthDate
        WHERE Source.IsBuyer = 1
    ) AS Source
    ON Target.PersonId = Source.PersonId
    WHEN NOT MATCHED THEN
        INSERT (PersonId, Phone)
        VALUES (Source.PersonId, Source.Phone)
    WHEN MATCHED THEN
        UPDATE SET Phone = Source.Phone;

    -- 5. Retornar PersonId y BuyerId (si aplica)
    SELECT 
        p.PersonId,
        ISNULL(b.BuyerId, 0) AS BuyerId
    FROM @Persons Source
    INNER JOIN dbo.Person p
        ON p.FirstName = Source.FirstName
        AND p.FirstLastName = Source.FirstLastName
        AND p.SecondLastName = Source.SecondLastName
        AND p.BirthDate = Source.BirthDate
    LEFT JOIN dbo.Buyer b
        ON b.PersonId = p.PersonId
    ORDER BY Source.IsBuyer DESC;  -- El buyer va primero
END;
GO