CREATE TABLE Aircraft (
  aircraftId uniqueidentifier NOT NULL,
  model varchar(15) unique NOT NULL,
  weight decimal(10, 4) NOT NULL,
  adminId uniqueidentifier NOT NULL,
  numberEconomyClassRows smallint check(numberEconomyClassRows >= 0),
  numberSeatingRowsEconomy smallint check(numberSeatingRowsEconomy >= 0),
  numberFirstClassRows smallint check(numberFirstClassRows >= 0),
  numberSeatingRowsFirst smallint check(numberSeatingRowsFirst >= 0),
  CONSTRAINT PK_Aicraft PRIMARY KEY(aircraftId),
  CONSTRAINT FK_User_AdminId FOREIGN KEY(adminId) REFERENCES User(UserId),
  CONSTRAINT CK_Model CHECK(model NOT LIKE '% %')
);