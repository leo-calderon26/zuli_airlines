USE ZuliAirlines;
GO

CREATE TABLE Person (
    PersonId UNIQUEIDENTIFIER PRIMARY KEY,
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

CREATE TABLE AirlineUser (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    PersonId UNIQUEIDENTIFIER NOT NULL UNIQUE,

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
    REFERENCES Person(PersonId),

    CONSTRAINT FK_AirlineUser_ManagedBy
    FOREIGN KEY (ManagedByAdminId)
    REFERENCES AirlineUser(UserId)
);
GO

--Add First User 
DECLARE @AdminPersonId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Person (
    PersonId,
    NationalId,
    FirstName,
    FirstLastName,
    SecondLastName,
    Email
)
VALUES (
    @AdminPersonId,
    '100000000',
    'Admin',
    'Zuli',
    'Airlines',
    NULL
);

INSERT INTO AirlineUser (
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
VALUES (
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