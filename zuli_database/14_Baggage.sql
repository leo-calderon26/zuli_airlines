CREATE TABLE dbo.Baggage (
    BaggageId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    PassengerId INT NOT NULL,
    ReservationId INT NOT NULL,
    Weight DECIMAL(6,2) NOT NULL CHECK (Weight > 0),
    Size VARCHAR(10) NOT NULL,
    Type VARCHAR(20),

    CONSTRAINT FK_Baggage_Person
    FOREIGN KEY (PassengerId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_Baggage_Reservation
    FOREIGN KEY (ReservationId)
    REFERENCES dbo.Reservation(ReservationId)
);
GO