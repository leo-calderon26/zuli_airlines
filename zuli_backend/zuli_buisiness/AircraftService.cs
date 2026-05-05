using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
using zuli_Business.Validation;

namespace zuli_Business
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
            if (!await _repository.IsAdmin(aircraft.AdminId))
            {
                throw new ZuliNotFoundException($"EL Usuario que esta intentando crear la aeronave y no tiene permisos {aircraft.AdminId}");
            }
            if (!string.IsNullOrEmpty(aircraft.model.ToLower()) && await _repository.AlreadyExistByModel(aircraft.model))
            {
                throw new ZuliNotFoundException($"Ya existe una aeronave con el mismo nombre {aircraft.model}");
            }
            _validator.ValidateAircraftInfo(aircraft);

            var newAircraft = new AircraftEntity
            {
                AdminId = aircraft.AdminId,
                model = aircraft.model,
                weight = aircraft.weight,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
                baggageCapacity = aircraft.baggageCapacity,
            };

            await _repository.CreateAircraft(newAircraft);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion de la aeronave correctamente",
            };
        }

        public async Task<IEnumerable<AircraftDTO>> GetAll()
        {
            var aircraft = await _repository.GetAll();

            return aircraft.Select(item => new AircraftDTO
                {
                    model = item.model,
                    weight = item.weight, 
                    baggageCapacity = item.baggageCapacity,
                    numberEconomyClassRows= item.numberEconomyClassRows,
                    numberSeatingRowsEconomy = item.numberSeatingRowsEconomy,
                    numberFirstClassRows = item.numberFirstClassRows,
                    numberSeatingRowsFirst = item.numberSeatingRowsFirst,
                }
            ).ToList();
        }

        public async Task<AircraftPaginatedResponseDTO<AircraftDTO>> GetAircraftsPaginated(int pageNumber, int pageSize)
        {
            var (aircrafts, totalCount) = await _repository.GetAircraftsPaginated(pageNumber, pageSize);
            
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var aircraftDTOs = aircrafts.Select(item => new AircraftDTO
            {
                model = item.model,
                weight = item.weight,
                baggageCapacity = item.baggageCapacity,
                numberEconomyClassRows = item.numberEconomyClassRows,
                numberSeatingRowsEconomy = item.numberSeatingRowsEconomy,
                numberFirstClassRows = item.numberFirstClassRows,
                numberSeatingRowsFirst = item.numberSeatingRowsFirst,
            }).ToList();

            return new AircraftPaginatedResponseDTO<AircraftDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                Data = aircraftDTOs
            };
        }
    }
}
