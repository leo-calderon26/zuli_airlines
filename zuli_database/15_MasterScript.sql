IF DB_ID('ZuliAirlines') IS NULL
BEGIN
    CREATE DATABASE ZuliAirlines;
END;
GO

USE ZuliAirlines;
GO

IF OBJECT_ID('dbo.PassengerReservation','U') IS NOT NULL DROP TABLE dbo.PassengerReservation;
IF OBJECT_ID('dbo.Baggage','U') IS NOT NULL DROP TABLE dbo.Baggage;
IF OBJECT_ID('dbo.Service','U') IS NOT NULL DROP TABLE dbo.Service;
IF OBJECT_ID('dbo.Passport','U') IS NOT NULL DROP TABLE dbo.Passport;
IF OBJECT_ID('dbo.Reservation','U') IS NOT NULL DROP TABLE dbo.Reservation;
IF OBJECT_ID('dbo.Flight','U') IS NOT NULL DROP TABLE dbo.Flight;
IF OBJECT_ID('dbo.Seat','U') IS NOT NULL DROP TABLE dbo.Seat;
IF OBJECT_ID('dbo.FlightRoute','U') IS NOT NULL DROP TABLE dbo.FlightRoute;
IF OBJECT_ID('dbo.Aircraft','U') IS NOT NULL DROP TABLE dbo.Aircraft;
IF OBJECT_ID('dbo.Airport','U') IS NOT NULL DROP TABLE dbo.Airport;
IF OBJECT_ID('dbo.AirlineUser','U') IS NOT NULL 
BEGIN
    ALTER TABLE dbo.AirlineUser DROP CONSTRAINT IF EXISTS FK_AirlineUser_ManagedBy;
    DROP TABLE dbo.AirlineUser;
END

IF OBJECT_ID('dbo.Itinerary','U') IS NOT NULL DROP TABLE dbo.Itinerary;
IF OBJECT_ID('dbo.Person','U') IS NOT NULL DROP TABLE dbo.Person;
IF OBJECT_ID('dbo.Airline','U') IS NOT NULL DROP TABLE dbo.Airline;
GO

CREATE TABLE dbo.Person
(
    PersonId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    NationalId CHAR(9) NOT NULL UNIQUE,
    FirstName VARCHAR(50) NOT NULL,
    FirstLastName VARCHAR(50) NOT NULL,
    SecondLastName VARCHAR(50) NOT NULL,
    Email VARCHAR(254) NULL,

    CONSTRAINT CK_Person_NationalId
    CHECK (
        LEN(NationalId) = 9
        AND NationalId NOT LIKE '%[^0-9]%'
    ),

    CONSTRAINT CK_Person_FirstName
    CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0),

    CONSTRAINT CK_Person_FirstLastName
    CHECK (LEN(LTRIM(RTRIM(FirstLastName))) > 0),

    CONSTRAINT CK_Person_SecondLastName
    CHECK (LEN(LTRIM(RTRIM(SecondLastName))) > 0),

    CONSTRAINT CK_Person_Email
    CHECK (
        Email IS NULL
        OR Email LIKE '%@%.%'
    )
);
GO

CREATE TABLE dbo.AirlineUser
(
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    PersonId INT NOT NULL UNIQUE,

    BusinessEmail VARCHAR(254) NOT NULL UNIQUE,
    BusinessId VARCHAR(50) NOT NULL UNIQUE,
    UserRole VARCHAR(20) NOT NULL,
    PasswordHash VARCHAR(500) NULL,

    IsActive BIT NOT NULL DEFAULT 0,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME NULL,

    ManagedByAdminId UNIQUEIDENTIFIER NULL,
    ActivationTokenHash VARCHAR(500) NULL,

    CONSTRAINT CK_AirlineUser_BusinessEmail
    CHECK (BusinessEmail LIKE '%@%.%'),

    CONSTRAINT CK_AirlineUser_UserRole
    CHECK (UserRole IN ('Operator', 'Administrator')),

    CONSTRAINT CK_AirlineUser_FailedLoginAttempts
    CHECK (FailedLoginAttempts >= 0),

    CONSTRAINT FK_AirlineUser_Person
    FOREIGN KEY (PersonId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_AirlineUser_ManagedBy
    FOREIGN KEY (ManagedByAdminId)
    REFERENCES dbo.AirlineUser(UserId)
);
GO

CREATE TABLE dbo.Airline
(
    AirlineId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AirlineName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Host VARCHAR(100),

    CONSTRAINT CK_AirlineEmail
    CHECK (Email LIKE '%@%.%')
);
GO

CREATE TABLE dbo.Itinerary
(
    ItineraryId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FinalDestination VARCHAR(100) NOT NULL,
    Type VARCHAR(30) NOT NULL,
    TicketAmount DECIMAL(10,2) CHECK (TicketAmount >= 0)
);
GO

CREATE TABLE dbo.Airport
(
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

CREATE TABLE dbo.Aircraft
(
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    Model VARCHAR(15) NOT NULL UNIQUE,
    Weight DECIMAL(10, 4) NOT NULL,
    BaggageCapacity DECIMAL(10, 4) NOT NULL,
    AdminId UNIQUEIDENTIFIER NOT NULL,
    NumberEconomyClassRows SMALLINT NOT NULL CHECK (NumberEconomyClassRows >= 0),
    NumberSeatingRowsEconomy SMALLINT NOT NULL CHECK (NumberSeatingRowsEconomy >= 0),
    NumberFirstClassRows SMALLINT NOT NULL CHECK (NumberFirstClassRows >= 0),
    NumberSeatingRowsFirst SMALLINT NOT NULL CHECK (NumberSeatingRowsFirst >= 0),

    CONSTRAINT PK_Aircraft
    PRIMARY KEY (AircraftId),

    CONSTRAINT FK_Aircraft_AdminId
    FOREIGN KEY (AdminId)
    REFERENCES dbo.AirlineUser(UserId),

    CONSTRAINT CK_Model
    CHECK (Model NOT LIKE '% %'),

    CONSTRAINT CK_BaggageCapacity_30Percent
    CHECK (BaggageCapacity = ROUND(Weight * 0.3, 4))
);
GO

CREATE TABLE dbo.FlightRoute
(
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

CREATE TABLE dbo.Seat
(
    SeatId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    RowNumber INT NOT NULL,
    SeatLetter VARCHAR(2) NOT NULL,
    Class VARCHAR(15) NOT NULL CHECK (Class IN ('Economy', 'FirstClass')),

    CONSTRAINT UQ_Seat_Plane
    UNIQUE (AircraftId, RowNumber, SeatLetter),

    CONSTRAINT FK_Seat_Aircraft
    FOREIGN KEY (AircraftId)
    REFERENCES dbo.Aircraft(AircraftId)
);
GO

CREATE TABLE dbo.Flight
(
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

CREATE TABLE Service
(
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    FlightId UNIQUEIDENTIFIER NOT NULL,
    Description VARCHAR(100) NOT NULL,
    CONSTRAINT FK_Service_Flight FOREIGN KEY (FlightId) REFERENCES Flight(Id)
);

CREATE TABLE dbo.Passport
(
    PassportNumber VARCHAR(20) NOT NULL PRIMARY KEY,
    PassengerId INT NOT NULL,
    DueDate DATE NOT NULL,

    CONSTRAINT FK_Passport_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId)
);
GO

CREATE TABLE dbo.Reservation
(
    ReservationId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ReservationCode VARCHAR(20) NOT NULL UNIQUE,
    ReservationOrigin VARCHAR(100),
    TotalPayment DECIMAL(12,2) CHECK (TotalPayment >= 0),
    PurchaseDate DATE NOT NULL,
    ClientId INT NOT NULL,
    ItineraryId INT NOT NULL,

    CONSTRAINT FK_Reservation_Client
    FOREIGN KEY (ClientId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_Reservation_Itinerary
    FOREIGN KEY (ItineraryId)
    REFERENCES dbo.Itinerary(ItineraryId)
);
GO

CREATE TABLE dbo.PassengerReservation
(
    PassengerId INT NOT NULL,
    ReservationId INT NOT NULL,

    CONSTRAINT PK_PassengerReservation
    PRIMARY KEY (PassengerId, ReservationId),

    CONSTRAINT FK_PassRes_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_PassRes_Reservation
    FOREIGN KEY (ReservationId)
    REFERENCES dbo.Reservation(ReservationId)
);
GO

CREATE TABLE dbo.Baggage
(
    BaggageId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    PassengerId INT NOT NULL,
    ReservationId INT NOT NULL,
    Weight DECIMAL(6,2) NOT NULL CHECK (Weight > 0),
    Size VARCHAR(10) NOT NULL,
    Type VARCHAR(20),

    CONSTRAINT FK_Baggage_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_Baggage_Reservation
    FOREIGN KEY (ReservationId)
    REFERENCES dbo.Reservation(ReservationId)
);
GO


DECLARE @AdminPersonId INT;
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Person
    (
    NationalId,
    FirstName,
    FirstLastName,
    SecondLastName,
    Email
    )
VALUES
    (
        '100000000',
        'Admin',
        'Zuli',
        'Airlines',
        NULL
);

SET @AdminPersonId = CONVERT(INT, SCOPE_IDENTITY());

INSERT INTO Itinerary
    (FinalDestination, Type, TicketAmount)
VALUES
    ('Madrid', 'Directo', 220.00),
    ('Bogota', 'Conexion', 180.50),
    ('Miami', 'Directo', 350.00),
    ('Buenos Aires', 'Conexion', 410.75),
    ('Ciudad de Mexico', 'Directo', 290.00);

INSERT INTO dbo.AirlineUser
    (
    UserId,
    PersonId,
    BusinessEmail,
    BusinessId,
    UserRole,
    PasswordHash,
    IsActive,
    FailedLoginAttempts,
    LockoutEnd,
    ManagedByAdminId,
    ActivationTokenHash
    )
VALUES
    (
        @AdminUserId,
        @AdminPersonId,
        'admin@zuliairlines.com',
        '100000000',
        'Administrator',
        'AQAAAAIAAYagAAAAELNnl2iB8RxtYXy4vEboWswGhjV15KvSIP4gDUhFUN3y47P+sfKsVWppRF4NWc4fMQ==',
        1,
        0,
        NULL,
        NULL,
        NULL
);
GO


