CREATE TABLE dbo.Airline (
    AirlineId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AirlineName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Host VARCHAR(100),

    CONSTRAINT CK_AirlineEmail
    CHECK (Email LIKE '%@%.%')
);
GO