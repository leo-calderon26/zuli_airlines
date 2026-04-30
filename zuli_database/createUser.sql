USE ZuliAirlines

CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    BusinessEmail VARCHAR(100) NOT NULL UNIQUE,
    BusinessId VARCHAR(30) NOT NULL UNIQUE,
    UserRole VARCHAR(20) NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL,

    CONSTRAINT CK_User_UserRole
    CHECK (UserRole IN ('Operator', 'Administrator')),

    CONSTRAINT CK_User_BusinessEmail
    CHECK (BusinessEmail LIKE '%_@_%._%')
);