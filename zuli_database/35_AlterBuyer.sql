USE ZuliAirlines;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Buyer_Person')
BEGIN
    ALTER TABLE dbo.Buyer
    ADD CONSTRAINT FK_Buyer_Person
    FOREIGN KEY (PersonId) REFERENCES dbo.Person(PersonId);
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Buyer') AND name = 'FirstName')
    ALTER TABLE dbo.Buyer DROP COLUMN FirstName;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Buyer') AND name = 'FirstLastName')
    ALTER TABLE dbo.Buyer DROP COLUMN FirstLastName;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Buyer') AND name = 'SecondLastName')
    ALTER TABLE dbo.Buyer DROP COLUMN SecondLastName;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Buyer') AND name = 'Email')
    ALTER TABLE dbo.Buyer DROP COLUMN Email;
GO
