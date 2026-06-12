use ZuliAirlines
GO

CREATE TYPE dbo.PersonBulkType AS TABLE (
    FirstName VARCHAR(50),
    FirstLastName VARCHAR(50),
    SecondLastName VARCHAR(50),
    BirthDate VARCHAR(10),
    Gender VARCHAR(10),
    Email VARCHAR(254),
    PassportCountry VARCHAR(60),
    PassportDueDate DATE
);