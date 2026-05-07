CREATE TABLE dbo.PassengerReservation (
    PassengerId INT NOT NULL,
    ReservationId INT NOT NULL,

    CONSTRAINT PK_PassengerReservation
    PRIMARY KEY (PassengerId, ReservationId),

    CONSTRAINT FK_PassRes_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_PassRes_Reservation
    FOREIGN KEY (ReservationId)
    REFERENCES dbo.Reservation(ReservationId)
);
GO