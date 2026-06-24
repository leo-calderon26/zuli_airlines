CREATE Procedure dbo.sp_HandleAircraftDeletion
	@selectedAircraftToDelete uniqueidentifier
AS
BEGIN
	DECLARE @aircraftSoftDelete bit
	DECLARE @currentFlightRoute int

	set @aircraftSoftDelete = 0

	DECLARE aircraft_softDelete_cursor CURSOR FOR 
	SELECT distinct fr.FlightRouteId from Aircraft ac 
	inner join FlightRoute fr on ac.AircraftId = fr.AircraftId 
	inner join Flight f on fr.FlightRouteId = f.FlightRouteId 
	where ac.AircraftId = @selectedAircraftToDelete
 
	OPEN aircraft_softDelete_cursor
 
	FETCH NEXT FROM aircraft_softDelete_cursor INTO @currentFlightRoute  
 
	WHILE @@FETCH_STATUS = 0  

	BEGIN  
	 set @aircraftSoftDelete = 1
	 UPDATE FlightRoute set Status = 'Deshabilitada' where FlightRouteId = @currentFlightRoute
 
	 FETCH NEXT FROM aircraft_softDelete_cursor INTO @currentFlightRoute 
	END 
 
	CLOSE aircraft_softDelete_cursor
 
	DEALLOCATE aircraft_softDelete_cursor

	-- Se hace soft delete sobre aircraft si hubieron rutas con compras hechas
	UPDATE Aircraft set Status =
	CASE
		WHEN @aircraftSoftDelete = 1 THEN 'No Disponible'
		ELSE 'Disponible'
	END
	WHERE AircraftId = @selectedAircraftToDelete

	DECLARE aircraft_hardDelete_cursor CURSOR FOR 

	select fr.FlightRouteId from Aircraft ac 
	inner join FlightRoute fr on ac.AircraftId = fr.AircraftId 
	where ac.AircraftId = @selectedAircraftToDelete AND fr.Status = 'Habilitada'

	OPEN aircraft_hardDelete_cursor

	FETCH NEXT FROM aircraft_hardDelete_cursor INTO @currentFlightRoute  

	WHILE @@FETCH_STATUS = 0

	BEGIN
	 DELETE FlightRoute where FlightRouteId = @currentFlightRoute
 
	 FETCH NEXT FROM aircraft_hardDelete_cursor INTO @currentFlightRoute 
	END 
 
	CLOSE aircraft_hardDelete_cursor
 
	DEALLOCATE aircraft_hardDelete_cursor

	DELETE Aircraft
	WHERE AircraftId = @selectedAircraftToDelete AND Status = 'Disponible'
END
GO