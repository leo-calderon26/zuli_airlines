CREATE TABLE FlighRoute(
flightRouteId int IDENTITY NOT NULL,
adminId uniqueidentifier NOT NULL,
airlineId int NOT NULL,
arrivalAirport varchar(3) NOT NULL,
departureAirport varchar(3) NOT NULL,
scheduledArrivalTime DateTime NOT NULL,
scheduledDeparture DateTime NOT NULL,
frequency int NOT NULL,
estimatedDuration int NOT NULL,
CONSTRAINT PK_flightRouteId PRIMARY KEY (flightRouteId),
CONSTRAINT FK_Airline_AirlineId FOREIGN KEY(airlineId) REFERENCES Airline(AirlineId),
CONSTRAINT FK_Airport_ArrivalAirport FOREIGN KEY(arrivalAirport) REFERENCES Airport(AirportCode),
CONSTRAINT FK_Airport_DepartureAirport FOREIGN KEY(departureAirport) REFERENCES Airport(AirportCode),
CONSTRAINT FK_User_AdminId FOREIGN KEY(adminId) REFERENCES [User](UserId),
CONSTRAINT CK_Frequency CHECK(frequency > 0)
)