ALTER TABLE FlightRoute DROP CONSTRAINT FK_FlightRoute_Aircraft

ALTER TABLE FlightRoute ADD CONSTRAINT FK_FlightRoute_Aircraft
FOREIGN KEY (AircraftId) REFERENCES dbo.Aircraft(AircraftId) ON DELETE CASCADE