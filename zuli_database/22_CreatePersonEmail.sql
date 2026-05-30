CREATE TABLE dbo.PersonEmail (
    PersonId INT NOT NULL PRIMARY KEY,
    Email VARCHAR(254) NOT NULL,

    CONSTRAINT FK_PersonEmail_Person
    FOREIGN KEY (PersonId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT CK_PersonEmail_Email
    CHECK (Email LIKE '%@%.%')
);
GO

INSERT INTO dbo.PersonEmail (PersonId, Email)
SELECT PersonId, Email
FROM dbo.Person
WHERE Email IS NOT NULL;
GO
ALTER TABLE dbo.Person DROP CONSTRAINT CK_Person_Email;
ALTER TABLE dbo.Person DROP COLUMN Email;
GO
