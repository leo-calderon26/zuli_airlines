USE ZuliAirlines;
GO

ALTER TABLE dbo.Flight
DROP CONSTRAINT FK_Flight_Itinerary;
GO

ALTER TABLE dbo.Flight
DROP COLUMN ItineraryId;
GO

CREATE TABLE dbo.ItineraryFlight
(
    ItineraryId INT NOT NULL,
    FlightId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_ItineraryFlight 
    PRIMARY KEY (ItineraryId, FlightId),

    CONSTRAINT FK_ItineraryFlight_Itinerary 
    FOREIGN KEY (ItineraryId) 
    REFERENCES dbo.Itinerary(ItineraryId),

    CONSTRAINT FK_ItineraryFlight_Flight 
    FOREIGN KEY (FlightId) 
    REFERENCES dbo.Flight(Id)
);
GO