Create Database ZuliAzirlines
use ZuliAzirlines
CREATE TABLE Aircraft (
  aircraftId int PRIMARY KEY NOT NULL,
  numberEconomyClassRows smallint check(numberEconomyClassRows >= 0),
  numberSeatingRowsEconomy smallint check(numberSeatingRowsEconomy >= 0),
  numberFirstClassRows smallint check(numberFirstClassRows >= 0),
  numberSeatingRowsFirst smallint check(numberSeatingRowsFirst >= 0),
  model varchar(15) check (model NOT LIKE '% %'),
  weight decimal(10, 2)
);
