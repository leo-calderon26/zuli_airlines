CREATE OR ALTER PROCEDURE dbo.sp_UpsertPersonBulk
    @Persons dbo.PersonBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Insertar/actualizar personas
    ;WITH NumberedPersons AS (
        SELECT *,
               ROW_NUMBER() OVER (
                   PARTITION BY FirstName, FirstLastName, SecondLastName, BirthDate
                   ORDER BY RowIndex
               ) AS rn
        FROM @Persons
    ),
    UniquePersons AS (
        SELECT FirstName, FirstLastName, SecondLastName, BirthDate, Gender
        FROM NumberedPersons
        WHERE rn = 1
    )
    MERGE INTO dbo.Person AS Target
    USING UniquePersons AS Source
    ON Target.FirstName = Source.FirstName
        AND Target.FirstLastName = Source.FirstLastName
        AND Target.SecondLastName = Source.SecondLastName
        AND Target.BirthDate = Source.BirthDate
    WHEN NOT MATCHED THEN
        INSERT (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
        VALUES (Source.FirstName, Source.FirstLastName, Source.SecondLastName, Source.BirthDate, Source.Gender);

    --Insertar/actualizar correos electronicos
    MERGE INTO dbo.PersonEmail AS Target
    USING (
        SELECT DISTINCT p.PersonId, Source.Email
        FROM @Persons Source
        INNER JOIN dbo.Person p
            ON p.FirstName = Source.FirstName
            AND p.FirstLastName = Source.FirstLastName
            AND p.SecondLastName = Source.SecondLastName
            AND p.BirthDate = Source.BirthDate
        WHERE Source.Email IS NOT NULL
    ) AS Source
    ON Target.PersonId = Source.PersonId
    WHEN NOT MATCHED THEN
        INSERT (PersonId, Email) VALUES (Source.PersonId, Source.Email)
    WHEN MATCHED THEN
        UPDATE SET Email = Source.Email;

    --Insertar pasaportes (solo para pasajeros, no buyers)
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

    --Insertar/actualizar Buyers (IsBuyer = 1)
    MERGE INTO dbo.Buyer AS Target
    USING (
        SELECT DISTINCT p.PersonId, Source.Phone
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
        INSERT (PersonId, Phone) VALUES (Source.PersonId, Source.Phone)
    WHEN MATCHED THEN
        UPDATE SET Phone = Source.Phone;

    --Retornar el personId y buyerId para cada fila de entrada
    SELECT 
        Source.RowIndex,
        p.PersonId,
        ISNULL(b.BuyerId, 0) AS BuyerId,
        Source.IsBuyer
    FROM @Persons Source
    INNER JOIN dbo.Person p
        ON p.FirstName = Source.FirstName
        AND p.FirstLastName = Source.FirstLastName
        AND p.SecondLastName = Source.SecondLastName
        AND p.BirthDate = Source.BirthDate
    LEFT JOIN dbo.Buyer b
        ON b.PersonId = p.PersonId
    ORDER BY Source.RowIndex;
END;
GO