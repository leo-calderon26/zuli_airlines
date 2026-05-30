CREATE TABLE dbo.Seat (
    SeatId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    RowNumber INT NOT NULL,
    SeatLetter VARCHAR(2) NOT NULL,
    Class VARCHAR(15) NOT NULL CHECK (Class IN ('Economy', 'FirstClass')),

    CONSTRAINT UQ_Seat_Plane
    UNIQUE (AircraftId, RowNumber, SeatLetter),

    CONSTRAINT FK_Seat_Aircraft
    FOREIGN KEY (AircraftId)
    REFERENCES dbo.Aircraft(AircraftId)
);
GO