DROP TABLE dbo.ItineraryFlight;
GO
ALTER TABLE dbo.Reservation
DROP CONSTRAINT FK_Reservation_Itinerary;
GO
ALTER TABLE dbo.Reservation
DROP COLUMN ItineraryId;
GO
DROP TABLE dbo.Itinerary;
GO
