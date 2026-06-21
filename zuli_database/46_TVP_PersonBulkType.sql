use ZuliAirlines
GO
CREATE TYPE dbo.PersonBulkType AS TABLE (
    RowIndex INT NOT NULL,
    FirstName VARCHAR(50),
    FirstLastName VARCHAR(50),
    SecondLastName VARCHAR(50),
    BirthDate VARCHAR(10),
    Gender VARCHAR(10),
    Email VARCHAR(254),
    PassportCountry VARCHAR(100),
    PassportDueDate DATETIME2,
    IsBuyer BIT,
    Phone VARCHAR(20)
);
GO


