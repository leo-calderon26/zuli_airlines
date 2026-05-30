CREATE TABLE dbo.Person (
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