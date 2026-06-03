USE ZuliAirlines;
GO

SET NOCOUNT ON;

DECLARE @AdminBusinessId VARCHAR(50) = '100000000';
DECLARE @AdminEmail VARCHAR(254) = 'admin@zuliairlines.com';
DECLARE @AdminUserId UNIQUEIDENTIFIER;

SELECT @AdminUserId = UserId
FROM dbo.AirlineUser
WHERE BusinessId = @AdminBusinessId;

IF @AdminUserId IS NULL
BEGIN
    DECLARE @PersonId INT;

    INSERT INTO dbo.Person (FirstName, FirstLastName, SecondLastName, BirthDate, Gender)
    VALUES ('Admin', 'Zuli', 'Airlines', '', '');

    SET @PersonId = CAST(SCOPE_IDENTITY() AS INT);

    IF NOT EXISTS (SELECT 1 FROM dbo.PersonEmail WHERE PersonId = @PersonId)
    BEGIN
        INSERT INTO dbo.PersonEmail (PersonId, Email)
        VALUES (@PersonId, @AdminEmail);
    END

    SET @AdminUserId = NEWID();

    INSERT INTO dbo.AirlineUser
    (
        UserId,
        PersonId,
        NationalId,
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
        @PersonId,
        '100000000',
        @AdminEmail,
        @AdminBusinessId,
        'Administrator',
        NULL,
        1,
        0,
        NULL,
        NULL,
        NULL
    );
END

IF @AdminUserId IS NULL
BEGIN
    SELECT TOP 1 @AdminUserId = UserId
    FROM dbo.AirlineUser
    WHERE UserRole = 'Administrator';
END

IF @AdminUserId IS NULL
BEGIN
    THROW 50000, 'Admin user not found or created.', 1;
END

DECLARE @AirlineId INT;

SELECT @AirlineId = AirlineId
FROM dbo.Airline
WHERE AirlineName = 'zuliAirline';

IF @AirlineId IS NULL
BEGIN
    INSERT INTO dbo.Airline (AirlineName, Email, Phone, Host)
    VALUES ('zuliAirline', 'contacto@zuliairline.com', '555-1234', 'zuliairline.com');

    SET @AirlineId = CAST(SCOPE_IDENTITY() AS INT);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'SJO')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('SJO', 'Juan Santamaria International', 'Costa Rica', 'San Jose', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'FRA')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('FRA', 'Frankfurt Airport', 'Germany', 'Frankfurt', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'ORD')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('ORD', 'O Hare International', 'United States', 'Chicago', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'MEL')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('MEL', 'Melbourne Airport', 'Australia', 'Melbourne', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'MSQ')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('MSQ', 'Minsk National Airport', 'Belarus', 'Minsk', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'CDG')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('CDG', 'Charles de Gaulle', 'France', 'Paris', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'MAD')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('MAD', 'Adolfo Suarez Madrid-Barajas', 'Spain', 'Madrid', @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM dbo.Airport WHERE AirportCode = 'AMS')
    INSERT INTO dbo.Airport (AirportCode, Name, Country, City, AdminId)
    VALUES ('AMS', 'Schiphol', 'Netherlands', 'Amsterdam', @AdminUserId);

DECLARE @PiAircraftId UNIQUEIDENTIFIER;
DECLARE @AirbusAircraftId UNIQUEIDENTIFIER;

SELECT @PiAircraftId = AircraftId
FROM dbo.Aircraft
WHERE Model = 'PI-Sprint2';

IF @PiAircraftId IS NULL
BEGIN
    SET @PiAircraftId = NEWID();
    INSERT INTO dbo.Aircraft
    (
        AircraftId,
        Model,
        Weight,
        BaggageCapacity,
        AdminId,
        NumberEconomyClassRows,
        NumberSeatingRowsEconomy,
        NumberFirstClassRows,
        NumberSeatingRowsFirst
    )
    VALUES
    (
        @PiAircraftId,
        'PI-Sprint2',
        3000,
        900,
        @AdminUserId,
        27,
        6,
        3,
        4
    );
END

SELECT @AirbusAircraftId = AircraftId
FROM dbo.Aircraft
WHERE Model = 'AIRBUS-2026';

IF @AirbusAircraftId IS NULL
BEGIN
    SET @AirbusAircraftId = NEWID();
    INSERT INTO dbo.Aircraft
    (
        AircraftId,
        Model,
        Weight,
        BaggageCapacity,
        AdminId,
        NumberEconomyClassRows,
        NumberSeatingRowsEconomy,
        NumberFirstClassRows,
        NumberSeatingRowsFirst
    )
    VALUES
    (
        @AirbusAircraftId,
        'AIRBUS-2026',
        2800,
        840,
        @AdminUserId,
        20,
        4,
        2,
        2
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'SJO'
      AND ArrivalAirport = 'FRA'
      AND ScheduledDepartureTime = '07:00:00'
      AND ScheduledArrivalTime = '23:00:00'
      AND Frequency = 9
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'FRA',
        'SJO',
        '23:00:00',
        '07:00:00',
        9,
        32400,
        @PiAircraftId,
        0,
        100,
        800,
        2000,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'SJO'
      AND ArrivalAirport = 'ORD'
      AND ScheduledDepartureTime = '08:00:00'
      AND ScheduledArrivalTime = '14:00:00'
      AND Frequency = 72
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'ORD',
        'SJO',
        '14:00:00',
        '08:00:00',
        72,
        18000,
        @PiAircraftId,
        0,
        75,
        400,
        1000,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'ORD'
      AND ArrivalAirport = 'FRA'
      AND ScheduledDepartureTime = '11:00:00'
      AND ScheduledArrivalTime = '23:00:00'
      AND Frequency = 41
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'FRA',
        'ORD',
        '23:00:00',
        '11:00:00',
        41,
        25200,
        @PiAircraftId,
        0,
        75,
        450,
        1050,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'ORD'
      AND ArrivalAirport = 'FRA'
      AND ScheduledDepartureTime = '17:00:00'
      AND ScheduledArrivalTime = '05:00:00'
      AND Frequency = 41
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'FRA',
        'ORD',
        '05:00:00',
        '17:00:00',
        41,
        25200,
        @PiAircraftId,
        0,
        75,
        350,
        800,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'FRA'
      AND ArrivalAirport = 'MEL'
      AND ScheduledDepartureTime = '05:00:00'
      AND ScheduledArrivalTime = '08:00:00'
      AND Frequency = 16
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'MEL',
        'FRA',
        '08:00:00',
        '05:00:00',
        16,
        10800,
        @PiAircraftId,
        0,
        50,
        300,
        1000,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'MSQ'
      AND ArrivalAirport = 'SJO'
      AND ScheduledDepartureTime = '01:00:00'
      AND ScheduledArrivalTime = '04:00:00'
      AND Frequency = 8
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'SJO',
        'MSQ',
        '04:00:00',
        '01:00:00',
        8,
        36000,
        @AirbusAircraftId,
        0,
        100,
        950,
        2200,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'SJO'
      AND ArrivalAirport = 'CDG'
      AND ScheduledDepartureTime = '06:30:00'
      AND ScheduledArrivalTime = '22:30:00'
      AND Frequency = 9
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'CDG',
        'SJO',
        '22:30:00',
        '06:30:00',
        9,
        32400,
        @PiAircraftId,
        0,
        100,
        799,
        1999,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'SJO'
      AND ArrivalAirport = 'MAD'
      AND ScheduledDepartureTime = '06:45:00'
      AND ScheduledArrivalTime = '22:45:00'
      AND Frequency = 9
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'MAD',
        'SJO',
        '22:45:00',
        '06:45:00',
        9,
        32400,
        @PiAircraftId,
        0,
        100,
        790,
        1950,
        1.5,
        23.00
    );

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FlightRoute
    WHERE DepartureAirport = 'SJO'
      AND ArrivalAirport = 'AMS'
      AND ScheduledDepartureTime = '07:15:00'
      AND ScheduledArrivalTime = '23:15:00'
      AND Frequency = 9
)
    INSERT INTO dbo.FlightRoute
    (
        AdminId,
        AirlineId,
        ArrivalAirport,
        DepartureAirport,
        ScheduledArrivalTime,
        ScheduledDepartureTime,
        Frequency,
        EstimatedDuration,
        AircraftId,
        CarryOnPrice,
        CheckedPrice,
        TouristPrice,
        FirstClassPrice,
        CheckedBagMultiplier,
        MaxWeightPerBag
    )
    VALUES
    (
        @AdminUserId,
        @AirlineId,
        'AMS',
        'SJO',
        '23:15:00',
        '07:15:00',
        9,
        32400,
        @PiAircraftId,
        0,
        100,
        810,
        2050,
        1.5,
        23.00
    );
GO
