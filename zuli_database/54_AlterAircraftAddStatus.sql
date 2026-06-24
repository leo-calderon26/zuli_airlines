alter table dbo.Aircraft
add Status varchar(15) check (Status in ('Disponible', 'No Disponible')) 
default 'Disponible'