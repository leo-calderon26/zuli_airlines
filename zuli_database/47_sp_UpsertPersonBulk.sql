CREATE OR ALTER PROCEDURE dbo.sp_UpsertPersonBulk
    @Persons dbo.PersonBulkType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Insertar personas nuevas
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
        -- Si no existe, insertar a la nueva persona
        INSERT (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
        VALUES (source.FirstName, source.FirstLastName, source.SecondLastName, source.BirthDate, source.Gender);
    -- Insertar o actualizar correos electrónicos
    Merge INTO dbo.PersonEmail AS Target
    USING (
        SELECT p.PersonId, source.Email
        FROM @Persons source
        INNER JOIN dbo.Person p
            ON p.FirstName = source.FirstName
            AND p.FirstLastName = source.FirstLastName
            AND p.SecondLastName = source.SecondLastName
            AND p.BirthDate = source.BirthDate
            AND p.Gender = source.Gender
        Where source.Email IS NOT NULL        
    ) AS source
    ON target.PersonId = source.PersonId
    WHEN NOT MATCHED THEN
        INSERT (PersonId, Email)
        VALUES (source.PersonId, source.Email)
    WHEN MATCHED THEN
        UPDATE SET Email = source.Email;
    -- Insertar pasaportes
    Insert INTO dbo.Passport (PassengerId, PassportCountry, DueDate)
    SELECT p.PersonId, source.PassportCountry, source.PassportDueDate
    FROM @Persons source
    INNER JOIN dbo.Person p
        ON p.FirstName = source.FirstName
        AND p.FirstLastName = source.FirstLastName
        AND p.SecondLastName = source.SecondLastName
        AND p.BirthDate = source.BirthDate
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Passport pas
        WHERE pas.PassengerId = p.PersonId
        AND pas.PassportCountry = source.PassportCountry
    )
    -- Se retorna el numero de los pasajeros
    SELECT p.PersonId,
        source.FirstName,
        source.FirstLastName,
        source.SecondLastName,
        source.BirthDate
    FROM @Persons source
    INNER JOIN dbo.Person p
        ON p.FirstName = source.FirstName
        AND p.FirstLastName = source.FirstLastName
        AND p.SecondLastName = source.SecondLastName
        AND p.BirthDate = source.BirthDate;
END
