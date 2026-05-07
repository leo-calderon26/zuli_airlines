CREATE TABLE dbo.AirlineUser (
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