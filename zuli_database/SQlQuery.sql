
Create Database ZuliAzirlines
use ZuliAzirlines
Drop database Zuli
CREATE TABLE Aircraft (
  idAircraft int PRIMARY KEY NOT NULL,
  numberEconomyClassRows smallint check(numberEconomyClassRows >= 0),
  numberSeatingRowsEconomy smallint check(numberSeatingRowsEconomy >= 0),
  numberFirstClassRows smallint check(numberFirstClassRows >= 0),
  numberSeatingFirstRows smallint check(numberSeatingFirstRows >= 0),
  model varchar(15) check (model NOT LIKE '% %'),
  weight decimal(10, 2) check (weight < 1000) 
);
 