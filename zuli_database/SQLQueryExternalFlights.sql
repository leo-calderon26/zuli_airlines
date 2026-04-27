CREATE DATABASE zuli_airlines

CREATE TABLE flight(
	Id uniqueidentifier primary key NOT NULL,
	FlightDate DateTime,
	TouristPrice decimal(8, 2),
	FirstClassPrice decimal(8, 2),
	Status varchar(20),
	Duration int,
	RealArrivalTime datetime,
	CheckInStartTime datetime,
	CheckInDeadline datetime,
	RealDepartureTime datetime,
	CarryOnPrice decimal(8, 2),
	CheckedPrice decimal(8, 2),
	DepartureAirport varchar(3),
	ArrivalAirport varchar(3),
	CONSTRAINT fk_DepartureAirport FOREIGN KEY (DepartureAirport) REFERENCES airport(AirportCode),
	CONSTRAINT fk_ArrivalAirport FOREIGN KEY (ArrivalAirport) REFERENCES airport(AirportCode)
)

CREATE TABLE airport(
	AirportCode varchar(3) primary key NOT NULL,
	Name varchar(100),
	Country varchar(50),
	City varchar(50)
)

INSERT INTO airport (
	AirportCode, Name, Country, City
) VALUES ('SJO', 'Aeropuerto Juan Santamaria', 'Costa Rica', 'Alajuela')

INSERT INTO airport (
	AirportCode, Name, Country, City
) VALUES ('MEX', 'Aeropuerto Internacional de la Ciudad de Mexico', 'Mexico', 'Ciudad de Mexico')

ALTER TABLE

INSERT INTO flight(
	Id,
	FlightDate,
	TouristPrice,
	FirstClassPrice,
	Status,
	Duration,
	RealArrivalTime,
	CheckInStartTime,
	CheckInDeadline,
	RealDepartureTime,
	CarryOnPrice,
	CheckedPrice,
	DepartureAirport,
	ArrivalAirport
) VALUES (newId(), getdate(), 100.00, 200.00, 'Pending', 10800, getdate(), getdate(), getdate(), getdate(), 20.00, 10.00, 'SJO', 'MEX')

SELECT * FROM flight

SELECT f.Id flightGUID,
	                    f.RealDepartureTime departureTime,
	                    f.RealArrivalTime arrivalTime,
	                    f.Duration duration,
	                    a.AirportCode departureAirportCode,
	                    a.Name departureAirportName,
	                    a.City departureAirportCity,
	                    b.AirportCode arrivalAirportCode,
	                    b.Name arrivalAirportName,
	                    b.City arrivalAirportCity,
	                    f.TouristPrice touristPrice,
	                    f.FirstClassPrice firstClassPrice,
	                    f.CarryOnPrice carryOnPrice,
	                    f.CheckedPrice checkedPrice
	FROM flight f INNER JOIN Airport a ON f.DepartureAirport = a.AirportCode INNER JOIN Airport b ON f.ArrivalAirport = b.AirportCode
	WHERE f.DepartureAirport = 'SJO' AND f.ArrivalAirport = 'MEX' AND f.RealDepartureTime between '2026-04-23 15:00:56.937' AND '2026-04-23 15:00:56.938'

CREATE TABLE Aircraft (
  aircraftId int PRIMARY KEY NOT NULL,
  numberEconomyClassRows smallint check(numberEconomyClassRows >= 0),
  numberSeatingRowsEconomy smallint check(numberSeatingRowsEconomy >= 0),
  numberFirstClassRows smallint check(numberFirstClassRows >= 0),
  numberSeatingRowsFirst smallint check(numberSeatingRowsFirst >= 0),
  model varchar(15) check (model NOT LIKE '% %'),
  weight decimal(10, 2)
);