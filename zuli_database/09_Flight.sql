CREATE TABLE dbo.Flight (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Status VARCHAR(20) NOT NULL,
    FlightDate DATETIME NOT NULL,
    TouristPrice DECIMAL(10, 2) NOT NULL,
    FirstClassPrice DECIMAL(10, 2) NOT NULL,
    RealDepartureTime DATETIME NULL,
    RealArrivalTime DATETIME NULL,
    CheckInStartTime DATETIME NULL,
    CheckInDeadline DATETIME NULL,
    AirlineId INT NOT NULL,
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    ItineraryId INT NOT NULL,
    Duration INT NOT NULL,
    CarryOnPrice DECIMAL(10, 2) NULL,
    CheckedPrice DECIMAL(10, 2) NULL,
    AvailableSeats INT NOT NULL,
    AdminId UNIQUEIDENTIFIER NOT NULL,
    FlightRouteId INT NOT NULL,

    CONSTRAINT FK_Flight_Airline
    FOREIGN KEY (AirlineId)
    REFERENCES dbo.Airline(AirlineId),

    CONSTRAINT FK_Flight_Aircraft
    FOREIGN KEY (AircraftId)
    REFERENCES dbo.Aircraft(AircraftId),

    CONSTRAINT FK_Flight_Itinerary
    FOREIGN KEY (ItineraryId)
    REFERENCES dbo.Itinerary(ItineraryId),

    CONSTRAINT FK_Flight_Admin
    FOREIGN KEY (AdminId)
    REFERENCES dbo.AirlineUser(UserId),

    CONSTRAINT FK_Flight_FlightRoute
    FOREIGN KEY (FlightRouteId)
    REFERENCES dbo.FlightRoute(FlightRouteId),

    CONSTRAINT CK_TouristPrice
    CHECK (TouristPrice >= 0),

    CONSTRAINT CK_FirstClassPrice
    CHECK (FirstClassPrice >= 0),

    CONSTRAINT CK_Flight_Status
    CHECK (Status IN ('Programado', 'Confirmado', 'Demorado', 'Cancelado'))
);
GO