USE ZuliAirlines;
GO
CREATE TYPE dbo.PassengerCheckBulkType AS TABLE
(
    PassengerIndex INT NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    FirstLastName VARCHAR(50) NOT NULL,
    SecondLastName VARCHAR(50) NULL,
    BirthDate VARCHAR(10) NOT NULL,
    PassportCountry VARCHAR(60) NOT NULL
);
GO