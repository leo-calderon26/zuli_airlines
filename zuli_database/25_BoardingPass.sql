CREATE TABLE dbo.BoardingPass (
    FlightId UNIQUEIDENTIFIER NOT NULL,
    ReservationCode VARCHAR(20) NOT NULL,
    PassengerId INT NOT NULL,
    CONSTRAINT PK_BoardingPass
    PRIMARY KEY (FlightId, ReservationCode, PassengerId),
    CONSTRAINT FK_BP_Flight
    FOREIGN KEY (FlightId) REFERENCES dbo.Flight(Id),
    CONSTRAINT FK_BP_Person
    FOREIGN KEY (PassengerId) REFERENCES dbo.Person(PersonId),
    CONSTRAINT FK_BP_Reservation
    FOREIGN KEY (ReservationCode) REFERENCES dbo.Reservation(ReservationCode)
);