CREATE TABLE dbo.Aircraft (
    AircraftId UNIQUEIDENTIFIER NOT NULL,
    Model VARCHAR(15) NOT NULL UNIQUE,
    Weight DECIMAL(10, 4) NOT NULL,
    BaggageCapacity DECIMAL(10, 4) NOT NULL,
    AdminId UNIQUEIDENTIFIER NOT NULL,
    NumberEconomyClassRows SMALLINT NOT NULL CHECK (NumberEconomyClassRows >= 0),
    NumberSeatingRowsEconomy SMALLINT NOT NULL CHECK (NumberSeatingRowsEconomy >= 0),
    NumberFirstClassRows SMALLINT NOT NULL CHECK (NumberFirstClassRows >= 0),
    NumberSeatingRowsFirst SMALLINT NOT NULL CHECK (NumberSeatingRowsFirst >= 0),

    CONSTRAINT PK_Aircraft
    PRIMARY KEY (AircraftId),

    CONSTRAINT FK_Aircraft_AdminId
    FOREIGN KEY (AdminId)
    REFERENCES dbo.AirlineUser(UserId),

    CONSTRAINT CK_Model
    CHECK (Model NOT LIKE '% %'),

    CONSTRAINT CK_BaggageCapacity_30Percent
    CHECK (BaggageCapacity = ROUND(Weight * 0.3, 4))
);
GO