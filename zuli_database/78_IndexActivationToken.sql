CREATE NONCLUSTERED INDEX IX_AirlineUser_ActivationTokenHash 
ON dbo.AirlineUser (ActivationTokenHash) 
WHERE ActivationTokenHash IS NOT NULL;
