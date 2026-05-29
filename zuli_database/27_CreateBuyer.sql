USE ZuliAirlines;
GO

CREATE TABLE dbo.Buyer
(
    BuyerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Phone VARCHAR(20) NULL,
    FirstName VARCHAR(50) NULL,
    FirstLastName VARCHAR(50) NULL,
    SecondLastName VARCHAR(50) NULL,
    Email VARCHAR(254) NULL
);