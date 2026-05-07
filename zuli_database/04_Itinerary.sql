CREATE TABLE dbo.Itinerary (
    ItineraryId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FinalDestination VARCHAR(100) NOT NULL,
    Type VARCHAR(30) NOT NULL,
    TicketAmount DECIMAL(10,2) CHECK (TicketAmount >= 0)
);
GO