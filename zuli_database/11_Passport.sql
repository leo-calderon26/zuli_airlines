CREATE TABLE dbo.Passport (
    PassportNumber VARCHAR(20) NOT NULL PRIMARY KEY,
    PassengerId INT NOT NULL,
    DueDate DATE NOT NULL,

    CONSTRAINT FK_Passport_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId)
);
GO