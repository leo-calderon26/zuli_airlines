USE ZuliAirlines;
GO

CREATE TABLE AirlineUser (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,

    NationalId CHAR(9) NOT NULL UNIQUE,
    BusinessEmail VARCHAR(254) NOT NULL UNIQUE,
    BusinessId VARCHAR(50) NOT NULL UNIQUE,

    FirstName VARCHAR(50) NOT NULL,
    FirstLastName VARCHAR(50) NOT NULL,
    SecondLastName VARCHAR(50) NOT NULL,

    UserRole VARCHAR(20) NOT NULL,
    PasswordHash VARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 0,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL,

    ManagedByAdminId UNIQUEIDENTIFIER NULL,
    ActivationTokenHash VARCHAR(500) NULL,

    CONSTRAINT CK_AirlineUser_NationalId
    CHECK (
        LEN(NationalId) = 9
        AND NationalId NOT LIKE '%[^0-9]%'
    ),

    CONSTRAINT CK_AirlineUser_FirstName
    CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0),

    CONSTRAINT CK_AirlineUser_FirstLastName
    CHECK (LEN(LTRIM(RTRIM(FirstLastName))) > 0),

    CONSTRAINT CK_AirlineUser_SecondLastName
    CHECK (LEN(LTRIM(RTRIM(SecondLastName))) > 0),

    CONSTRAINT CK_AirlineUser_UserRole
    CHECK (UserRole IN ('Operator', 'Administrator')),

    CONSTRAINT CK_AirlineUser_BusinessEmail
    CHECK (BusinessEmail LIKE '%@%.%'),

    CONSTRAINT CK_AirlineUser_FailedLoginAttempts
    CHECK (FailedLoginAttempts >= 0),

    CONSTRAINT FK_AirlineUser_ManagedBy
    FOREIGN KEY (ManagedByAdminId)
    REFERENCES AirlineUser(UserId)
);
GO

--Add First User 
INSERT INTO AirlineUser (
    UserId,
    NationalId,
    BusinessEmail,
    BusinessId,
    FirstName,
    FirstLastName,
    SecondLastName,
    UserRole,
    PasswordHash,
    IsActive,
    FailedLoginAttempts,
    LockoutEnd,
    ManagedByAdminId,
    ActivationTokenHash
)
VALUES (
    NEWID(),
    '100000000',
    'admin@zuliairlines.com',
    '100000000',
    'Admin',
    'Zuli',
    'Airlines',
    'Administrator',
    'AQAAAAIAAYagAAAAELNnl2iB8RxtYXy4vEboWswGhjV15KvSIP4gDUhFUN3y47P+sfKsVWppRF4NWc4fMQ==',
    1,
    0,
    NULL,
    NULL,
    NULL
);
GO