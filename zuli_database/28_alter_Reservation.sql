GO
EXEC sp_rename 'dbo.Reservation.ClientId', 'BuyerId', 'COLUMN';
GO
ALTER TABLE dbo.Reservation ALTER COLUMN BuyerId INT NOT NULL;
GO
UPDATE r
SET r.BuyerId = b.BuyerId
FROM dbo.Reservation r
INNER JOIN dbo.Buyer b ON r.BuyerId = b.PersonId;
GO
ALTER TABLE dbo.Reservation
ADD CONSTRAINT FK_Reservation_Buyer
FOREIGN KEY (BuyerId) REFERENCES dbo.Buyer(BuyerId);
GO