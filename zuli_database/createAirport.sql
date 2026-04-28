USE ZuliAirlines

CREATE TABLE Airport (
    AirportCode char(3) NOT NULL,
    Name varchar(100) NOT NULL,
    Country varchar(60) NOT NULL,
    City varchar(60) NOT NULL,
    AdminId uniqueIdentifier NOT NULL,
    CONSTRAINT FK_Airport_Administrator 
    FOREIGN KEY (AdminId) 
    REFERENCES Administrator(UserId),
    PRIMARY KEY (AirportCode),
);