CREATE TABLE dbo.FlightRoute (
    FlightRouteId INT IDENTITY NOT NULL,
    AdminId UNIQUEIDENTIFIER NOT NULL,
    AirlineId INT NOT NULL,
    ArrivalAirport VARCHAR(3) NOT NULL,
    DepartureAirport VARCHAR(3) NOT NULL,
    ScheduledArrivalTime DATETIME NOT NULL,
    ScheduledDeparture DATETIME NOT NULL,
    Frequency INT NOT NULL,
    EstimatedDuration INT NOT NULL,

    CONSTRAINT PK_FlightRouteId
    PRIMARY KEY (FlightRouteId),

    CONSTRAINT FK_Airline_AirlineId
    FOREIGN KEY (AirlineId)
    REFERENCES dbo.Airline(AirlineId),

    CONSTRAINT FK_Airport_ArrivalAirport
    FOREIGN KEY (ArrivalAirport)
    REFERENCES dbo.Airport(AirportCode),

    CONSTRAINT FK_Airport_DepartureAirport
    FOREIGN KEY (DepartureAirport)
    REFERENCES dbo.Airport(AirportCode),

    CONSTRAINT FK_User_AdminId
    FOREIGN KEY (AdminId)
    REFERENCES dbo.AirlineUser(UserId),

    CONSTRAINT CK_Frequency
    CHECK (Frequency > 0)
);
GO