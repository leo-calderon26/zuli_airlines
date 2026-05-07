CREATE TABLE dbo.Reservation (
    ReservationId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ReservationCode VARCHAR(20) NOT NULL UNIQUE,
    ReservationOrigin VARCHAR(100),
    TotalPayment DECIMAL(12,2) CHECK (TotalPayment >= 0),
    PurchaseDate DATE NOT NULL,
    ClientId INT NOT NULL,
    ItineraryId INT NOT NULL,

    CONSTRAINT FK_Reservation_Client
    FOREIGN KEY (ClientId)
    REFERENCES dbo.Person(PersonId),

    CONSTRAINT FK_Reservation_Itinerary
    FOREIGN KEY (ItineraryId)
    REFERENCES dbo.Itinerary(ItineraryId)
);
GO