using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
using zuli_Buisiness.Validation;

namespace zuli_Buisiness
{
    public class AircraftService : IAircraftService
    {
        // Inyeccion de dependencias
        private readonly IAircraftRepository _repository;
        private readonly AircraftValidator _validator;
        public AircraftService(IAircraftRepository repository)
        {
            _repository = repository;
            _validator = new AircraftValidator();
        }

        public async Task<BasicResponseDTO> CreateAircraft(AircraftDTO aircraft) 
        {
            if (await _repository.AlreadyExist(aircraft.aircraftId))
            {
                throw new ZuliNotFoundException($"Se encontro una aeronave con el mismo id {aircraft.aircraftId}");
            }
            // TODO(randy): Preguntar si es necesario validar que el Id de la eronave
            if (!string.IsNullOrEmpty(aircraft.model.ToLower()) && await _repository.AlreadyExistByModel(aircraft.model))
            {
                throw new ZuliNotFoundException($"Ya existe una aeronave con el mismo nombre {aircraft.model}");
            }
            // aqui se tiene que llamar el 
            _validator.ValidateAircraftInfo(aircraft);

            var newAircraft = new AircraftEntity
            {
                AircraftId = aircraft.aircraftId,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
                model = aircraft.model,
                weight = aircraft.weight
            };

            await _repository.CreateAircraft(newAircraft);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion de la aeronave correctamente",
            };
        }
    }
}
