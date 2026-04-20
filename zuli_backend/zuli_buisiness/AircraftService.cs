using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;

namespace zuli_Buisiness
{
    public class AircraftService : IAircraftService
    {
        // Inyeccion de dependencias
        private readonly IAircraftRepository _repository;
        public AircraftService(IAircraftRepository repository) => _repository = repository;
        
        public async Task<AircraftDTO?> CreateAircraft(AircraftDTO aircraft) 
        {
            // TODO(randy) arreglar esto
            if(aircraft == null) throw new ArgumentNullException(nameof(aircraft));

            var newAircraft = new AircraftEntity
            {
                AircraftId = aircraft.AircraftId,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
                model = aircraft.model,
                weight = aircraft.weight


            };

            var aircraftCreate = await _repository.CreateAircraft(newAircraft);
            // TODO(randy) Cambiar esta manejo de error

            if(aircraftCreate is null)
            {
                throw new NotFoundException($"The aircraft with the id {newAircraft.AircraftId} cannot be created");
            }

            return new AircraftDTO
            {
                AircraftId = aircraft.AircraftId,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
                model = aircraft.model,
                weight = aircraft.weight

            };
        }
    }
}
