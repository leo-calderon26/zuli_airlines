CREATE TABLE dbo.Airport (
    AirportCode VARCHAR(3) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Country VARCHAR(60) NOT NULL,
    City VARCHAR(60) NOT NULL,
    AdminId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_Airport
    PRIMARY KEY (AirportCode),

    CONSTRAINT FK_Airport_Administrator
    FOREIGN KEY (AdminId)
    REFERENCES dbo.AirlineUser(UserId)
);
GO