USE ZuliAirlines;
GO

DECLARE @rootEmail VARCHAR(254) = 'admin@zuliairlines.com';

IF
(
    SELECT COUNT(*)
    FROM dbo.AirlineUser
    WHERE BusinessEmail = @rootEmail
) <> 1
BEGIN
    THROW 50010, 'No se encontró exactamente un usuario root con el correo configurado.', 1;
END;

IF EXISTS
(
    SELECT 1
    FROM dbo.AirlineUser
    WHERE BusinessEmail = @rootEmail
      AND UserRole <> 'Administrator'
)
BEGIN
    THROW 50011, 'El usuario root configurado debe tener rol Administrator.', 1;
END;

UPDATE dbo.AirlineUser
SET IsProtected = 1
WHERE BusinessEmail = @rootEmail;
GO