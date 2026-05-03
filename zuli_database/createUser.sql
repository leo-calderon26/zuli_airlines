USE ZuliAirlines

CREATE TABLE AirlineUser (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    BusinessEmail VARCHAR(254) NOT NULL UNIQUE,
    BusinessId VARCHAR(50) NOT NULL UNIQUE,
    UserRole VARCHAR(20) NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME NULL,

    ManagedByAdminId UNIQUEIDENTIFIER NULL,

    CONSTRAINT CK_User_UserRole
    CHECK (UserRole IN ('Operator', 'Administrator')),

    CONSTRAINT CK_UserBusinessEmail
    CHECK (BusinessEmail LIKE '%@%.%'),

    CONSTRAINT FK_User_ManagedBy FOREIGN KEY (ManagedByAdminId) 
    REFERENCES [User](UserId)
);