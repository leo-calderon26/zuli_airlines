IF DB_ID('ZuliAirlines') IS NULL
BEGIN
    CREATE DATABASE ZuliAirlines;
END;
GO
USE ZuliAirlines;
GO

IF OBJECT_ID('dbo.BoardingPass','U') IS NOT NULL DROP TABLE dbo.BoardingPass;
IF OBJECT_ID('dbo.Baggage','U') IS NOT NULL DROP TABLE dbo.Baggage;
IF OBJECT_ID('dbo.PassengerReservation','U') IS NOT NULL DROP TABLE dbo.PassengerReservation;
IF OBJECT_ID('dbo.Service','U') IS NOT NULL DROP TABLE dbo.Service;
IF OBJECT_ID('dbo.Passport','U') IS NOT NULL DROP TABLE dbo.Passport;
IF OBJECT_ID('dbo.Reservation','U') IS NOT NULL DROP TABLE dbo.Reservation;
IF OBJECT_ID('dbo.Buyer','U') IS NOT NULL DROP TABLE dbo.Buyer;
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
IF OBJECT_ID('dbo.PersonEmail','U') IS NOT NULL DROP TABLE dbo.PersonEmail;
IF OBJECT_ID('dbo.Person','U') IS NOT NULL DROP TABLE dbo.Person;
IF OBJECT_ID('dbo.Airline','U') IS NOT NULL DROP TABLE dbo.Airline;
GO

IF OBJECT_ID('dbo.getAircraftTotalSeats','FN') IS NOT NULL DROP FUNCTION dbo.getAircraftTotalSeats;
IF OBJECT_ID('dbo.sp_UpsertPerson','P') IS NOT NULL DROP PROCEDURE dbo.sp_UpsertPerson;
IF OBJECT_ID('dbo.sp_UpsertBuyer','P') IS NOT NULL DROP PROCEDURE dbo.sp_UpsertBuyer;
GO

CREATE TABLE dbo.Person
(
    PersonId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    FirstLastName VARCHAR(50) NOT NULL,
    SecondLastName VARCHAR(50) NOT NULL,
    BirthDate VARCHAR(10) NOT NULL DEFAULT '',
    Gender VARCHAR(20) NOT NULL DEFAULT '',
    CONSTRAINT CK_Person_FirstName
    CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0),
    CONSTRAINT CK_Person_FirstLastName
    CHECK (LEN(LTRIM(RTRIM(FirstLastName))) > 0),
    CONSTRAINT CK_Person_SecondLastName
    CHECK (LEN(LTRIM(RTRIM(SecondLastName))) > 0)
);
GO

CREATE TABLE dbo.PersonEmail
(
    PersonId INT NOT NULL PRIMARY KEY,
    Email VARCHAR(254) NOT NULL,
    CONSTRAINT FK_PersonEmail_Person
    FOREIGN KEY (PersonId)
    REFERENCES dbo.Person(PersonId),
    CONSTRAINT CK_PersonEmail_Email
    CHECK (Email LIKE '%@%.%')
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
    ScheduledArrivalTime TIME NOT NULL,
    ScheduledDepartureTime TIME NOT NULL,
    Frequency INT NOT NULL,
    EstimatedDuration INT NOT NULL,
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    CarryOnPrice DECIMAL(10, 2) NULL,
    CheckedPrice DECIMAL(10, 2) NULL,
    TouristPrice DECIMAL(10, 2) NULL,
    FirstClassPrice DECIMAL(10, 2) NULL,
    CheckedBagMultiplier DECIMAL(10, 2) NOT NULL DEFAULT 1.0,
    MaxWeightPerBag DECIMAL(6, 2) NOT NULL DEFAULT 23.00,
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
    CONSTRAINT FK_FlightRoute_Aircraft
    FOREIGN KEY (AircraftId)
    REFERENCES dbo.Aircraft(AircraftId),
    CONSTRAINT CK_Frequency
    CHECK (Frequency > 0),
    CONSTRAINT CK_FlightRoute_TouristPrice
    CHECK (TouristPrice >= 0),
    CONSTRAINT CK_FlightRoute_FirstClassPrice
    CHECK (FirstClassPrice >= 0)
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
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    Duration INT NOT NULL DEFAULT 0,
    CarryOnPrice DECIMAL(10, 2) NULL,
    CheckedPrice DECIMAL(10, 2) NULL,
    AvailableSeats INT NOT NULL,
    AdminId UNIQUEIDENTIFIER NOT NULL,
    FlightRouteId INT NOT NULL,
    RealArrivalAirport VARCHAR(3) NULL,
    RealDepartureAirport VARCHAR(3) NULL,
    CONSTRAINT FK_Flight_Aircraft
    FOREIGN KEY (AircraftId)
    REFERENCES dbo.Aircraft(AircraftId),
    CONSTRAINT FK_Flight_Admin
    FOREIGN KEY (AdminId)
    REFERENCES dbo.AirlineUser(UserId),
    CONSTRAINT FK_Flight_FlightRoute
    FOREIGN KEY (FlightRouteId)
    REFERENCES dbo.FlightRoute(FlightRouteId),
    CONSTRAINT FK_Flight_RealArrivalAirport
    FOREIGN KEY (RealArrivalAirport)
    REFERENCES dbo.Airport(AirportCode),
    CONSTRAINT FK_Flight_RealDepartureAirport
    FOREIGN KEY (RealDepartureAirport)
    REFERENCES dbo.Airport(AirportCode),
    CONSTRAINT CK_TouristPrice
    CHECK (TouristPrice >= 0),
    CONSTRAINT CK_FirstClassPrice
    CHECK (FirstClassPrice >= 0),
    CONSTRAINT CK_Flight_Status
    CHECK (Status IN ('Programado', 'Confirmado', 'Demorado', 'Cancelado')),
    CONSTRAINT CK_Flight_Duration
    CHECK (Duration >= 0),
    CONSTRAINT CK_Flight_CarryOnPrice
    CHECK (CarryOnPrice >= 0),
    CONSTRAINT CK_Flight_CheckedPrice
    CHECK (CheckedPrice >= 0)
);
GO

CREATE TABLE dbo.Service
(
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    FlightId UNIQUEIDENTIFIER NOT NULL,
    Description VARCHAR(100) NOT NULL,
    CONSTRAINT FK_Service_Flight
    FOREIGN KEY (FlightId)
    REFERENCES dbo.Flight(Id)
);
GO

CREATE TABLE dbo.Passport
(
    PassengerId INT NOT NULL,
    PassportCountry VARCHAR(60) NOT NULL DEFAULT '',
    DueDate DATE NOT NULL,
    CONSTRAINT PK_Passport
    PRIMARY KEY (PassengerId, PassportCountry),
    CONSTRAINT FK_Passport_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId)
);
GO

CREATE TABLE dbo.Buyer
(
    BuyerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PersonId INT NOT NULL,
    Phone VARCHAR(20) NULL,
    CONSTRAINT FK_Buyer_Person
    FOREIGN KEY (PersonId)
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
    BuyerId INT NOT NULL,
    CONSTRAINT FK_Reservation_Buyer
    FOREIGN KEY (BuyerId)
    REFERENCES dbo.Buyer(BuyerId)
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

CREATE TABLE dbo.BoardingPass
(
    FlightId UNIQUEIDENTIFIER NOT NULL,
    ReservationCode VARCHAR(20) NOT NULL,
    PassengerId INT NOT NULL,
    CONSTRAINT PK_BoardingPass
    PRIMARY KEY (FlightId, ReservationCode, PassengerId),
    CONSTRAINT FK_BP_Flight
    FOREIGN KEY (FlightId)
    REFERENCES dbo.Flight(Id),
    CONSTRAINT FK_BP_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId),
    CONSTRAINT FK_BP_Reservation
    FOREIGN KEY (ReservationCode)
    REFERENCES dbo.Reservation(ReservationCode)
);
GO

CREATE FUNCTION dbo.getAircraftTotalSeats (@AircraftId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @TotalSeats INT;
    SELECT @TotalSeats = ((NumberEconomyClassRows * NumberSeatingRowsEconomy) + (NumberFirstClassRows * NumberSeatingRowsFirst))
    FROM dbo.Aircraft
    WHERE AircraftId = @AircraftId;
    RETURN ISNULL(@TotalSeats, 0);
END;
GO

CREATE PROCEDURE dbo.sp_UpsertPerson
    @FirstName VARCHAR(50),
    @FirstLastName VARCHAR(50),
    @SecondLastName VARCHAR(50),
    @BirthDate VARCHAR(10),
    @Gender VARCHAR(10),
    @Email VARCHAR(254) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @PersonId INT;
    SELECT @PersonId = PersonId
    FROM dbo.Person
    WHERE FirstName = @FirstName
      AND FirstLastName = @FirstLastName
      AND SecondLastName = @SecondLastName
      AND BirthDate = @BirthDate;
    IF @PersonId IS NULL
    BEGIN
        INSERT INTO dbo.Person (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
        VALUES (@FirstName, @FirstLastName, @SecondLastName, @BirthDate, @Gender);
        SET @PersonId = SCOPE_IDENTITY();
    END
    IF @Email IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.PersonEmail WHERE PersonId = @PersonId)
            UPDATE dbo.PersonEmail SET Email = @Email WHERE PersonId = @PersonId;
        ELSE
            INSERT INTO dbo.PersonEmail (PersonId, Email) VALUES (@PersonId, @Email);
    END
    SELECT @PersonId AS PersonId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpsertBuyer
    @FirstName VARCHAR(50),
    @FirstLastName VARCHAR(50),
    @SecondLastName VARCHAR(50),
    @BirthDate VARCHAR(10),
    @Email VARCHAR(254),
    @Phone VARCHAR(20)
AS
BEGIN
    DECLARE @PersonId INT;

    SELECT @PersonId = PersonId
    FROM dbo.Person
    WHERE FirstName = @FirstName
      AND FirstLastName = @FirstLastName
      AND SecondLastName = @SecondLastName
      AND BirthDate = @BirthDate;

    IF @PersonId IS NULL
    BEGIN
        INSERT INTO dbo.Person (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
        VALUES (@FirstName, @FirstLastName, @SecondLastName, @BirthDate, '');

        SET @PersonId = SCOPE_IDENTITY();
    END

    IF @Email IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.PersonEmail WHERE PersonId = @PersonId)
            UPDATE dbo.PersonEmail SET Email = @Email WHERE PersonId = @PersonId;
        ELSE
            INSERT INTO dbo.PersonEmail (PersonId, Email) VALUES (@PersonId, @Email);
    END

    DECLARE @BuyerId INT;
    SELECT @BuyerId = BuyerId FROM dbo.Buyer WHERE PersonId = @PersonId;

    IF @BuyerId IS NOT NULL
    BEGIN
        UPDATE dbo.Buyer
        SET Phone = @Phone
        WHERE BuyerId = @BuyerId;

        SELECT @BuyerId AS BuyerId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.Buyer (PersonId, Phone)
        VALUES (@PersonId, @Phone);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS BuyerId;
    END
END
GO

DECLARE @AdminPersonId INT;
INSERT INTO dbo.Person (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
VALUES ('Admin', 'Zuli', 'Airlines', '', '');
SET @AdminPersonId = CONVERT(INT, SCOPE_IDENTITY());

DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
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

INSERT INTO dbo.Airline (AirlineName, Email, Phone, Host)
VALUES ('zuliAirline', 'contacto@zuliairline.com', '555-1234', 'zuliairline.com');
GO

SELECT 'Tablas creadas' AS Estado;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
GO

SELECT * FROM FlightRoute
SELECT * FROM Flight

USE Zuliairlines;
GO
DECLARE @FlightRouteId INT = 1;
DECLARE @AircraftId UNIQUEIDENTIFIER;
DECLARE @FlightDate DATE = '2026-06-01';
DECLARE @Monday BIT = 1;
SELECT @AircraftId = AircraftId FROM dbo.FlightRoute WHERE FlightRouteId = @FlightRouteId;
IF @AircraftId IS NULL
BEGIN
    PRINT 'Error: FlightRouteId no existe';
    RETURN;
END

IF NOT (@Monday = 1 AND (SELECT Frequency FROM dbo.FlightRoute WHERE FlightRouteId = @FlightRouteId) & 1 = 1)
BEGIN
    PRINT 'Error: La ruta no opera los lunes';
    RETURN;
END
INSERT INTO dbo.Flight
(
    Id, Status, FlightDate, TouristPrice, FirstClassPrice,
    RealDepartureTime, RealArrivalTime, AircraftId, Duration,
    CarryOnPrice, CheckedPrice, AvailableSeats, AdminId,
    FlightRouteId, RealArrivalAirport, RealDepartureAirport
)
SELECT
    NEWID(),
    'Programado',
    @FlightDate,
    TouristPrice,
    FirstClassPrice,
    CAST(@FlightDate AS DATETIME) + CAST(ScheduledDepartureTime AS DATETIME),
    CAST(@FlightDate AS DATETIME) + CAST(ScheduledArrivalTime AS DATETIME),
    fr.AircraftId,
    fr.EstimatedDuration,
    fr.CarryOnPrice,
    fr.CheckedPrice,
    dbo.getAircraftTotalSeats(fr.AircraftId),
    fr.AdminId,
    fr.FlightRouteId,
    fr.DepartureAirport,
    fr.ArrivalAirport
FROM dbo.FlightRoute fr
WHERE fr.FlightRouteId = @FlightRouteId;
IF @@ROWCOUNT > 0
    PRINT 'Vuelo creado exitosamente para el lunes ' + CAST(@FlightDate AS VARCHAR(10));
ELSE
    PRINT 'Error al crear el vuelo';
GO

SELECT * FROM Buyer
SELECT * FROM Reservation
SELECT * FROM PersonEmail
SELECT * FROM PassengerReservation
SELECT * FROM Passport
SELECT * FROM Person
SELECT * FROM BoardingPass
SELECT * FROM BoardingPass WHERE ReservationCode ='OOHKTEVR'; 




ALTER TABLE dbo.Reservation ADD FlightClass VARCHAR(20) NULL;
ALTER TABLE dbo.Reservation ADD PaymentMethod VARCHAR(30) NULL;
