alter table dbo.FlightRoute
add Status varchar(15) check (Status in ('Habilitada', 'Deshabilitada')) 
default 'Habilitada'