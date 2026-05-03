CREATE TABLE Aircraft (
  aircraftId uniqueidentifier NOT NULL,
  model varchar(15) unique NOT NULL,
  weight decimal(10, 4) NOT NULL,
  baggageCapacity decimal(10, 4) NOT NULL,
  adminId uniqueidentifier NOT NULL,
  numberEconomyClassRows smallint check(numberEconomyClassRows >= 0),
  numberSeatingRowsEconomy smallint check(numberSeatingRowsEconomy >= 0),
  numberFirstClassRows smallint check(numberFirstClassRows >= 0),
  numberSeatingRowsFirst smallint check(numberSeatingRowsFirst >= 0),
  CONSTRAINT PK_Aicraft PRIMARY KEY(aircraftId),
  CONSTRAINT FK_User_AdminId FOREIGN KEY(adminId) REFERENCES [User](UserId),
  CONSTRAINT CK_Model CHECK(model NOT LIKE '% %'),
  CONSTRAINT CK_BaggageCapacity_30Percent CHECK (baggageCapacity = ROUND(weight * 0.3, 4))
);

CREATE TABLE Seat (
    SeatId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    RowNumber INT NOT NULL,
    SeatLetter VARCHAR(2) NOT NULL,
    Class VARCHAR(15) NOT NULL CHECK (Class IN ('Economy', 'FirstClass')),
    CONSTRAINT UQ_Seat_Plane UNIQUE (AircraftId, RowNumber, SeatLetter),
    CONSTRAINT FK_Seat_Aircraft FOREIGN KEY (AircraftId) REFERENCES Aircraft(AircraftId)
);