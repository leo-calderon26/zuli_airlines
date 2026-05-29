ALTER TABLE dbo.Passport
ADD PassportCountry VARCHAR(60) NOT NULL DEFAULT '';
GO
GO
ALTER TABLE dbo.Passport
DROP CONSTRAINT PK_Passport;
GO
ALTER TABLE dbo.Passport
DROP COLUMN PassportNumber;
ALTER TABLE dbo.Passport
ADD CONSTRAINT PK_Passport PRIMARY KEY (PassengerId, PassportCountry);