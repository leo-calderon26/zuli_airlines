using System;
using System.Collections.Generic;
using System.Text;
using Mapster;
using FluentValidation;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class AircraftService : IAircraftService
    {
        // Inyeccion de dependencias
        private readonly IAircraftRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly FluentValidation.IValidator<AircraftDTO> _validator;

        public AircraftService(
            IAircraftRepository repository,
            IUserRepository userRepository,
            FluentValidation.IValidator<AircraftDTO> validator)
        {
            _repository = repository;
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<BasicResponseDTO> CreateAircraft(AircraftDTO aircraft)
        {
            var validationResult = await _validator.ValidateAsync(aircraft);
            validationResult.ThrowIfInvalid();

            if (!await _userRepository.IsAdmin(aircraft.businessId))
            {
                throw new ZuliNotFoundException($"El Usuario que esta intentando crear la aeronave y no tiene permisos {aircraft.businessId}");
            }

            var userId = await _userRepository.GetUserId(aircraft.businessId);

            var newAircraft = aircraft.Adapt<AircraftEntity>();
            newAircraft.AdminId = userId;

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

            return aircraft.Select(item => item.Adapt<AircraftDTO>()).ToList();
        }

        public async Task<AircraftPaginatedResponseDTO<AircraftDTO>> GetAircraftsPaginated(int pageNumber, int pageSize)
        {
            var (aircrafts, totalCount) = await _repository.GetAircraftsPaginated(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var aircraftDTOs = aircrafts.Select(item => item.Adapt<AircraftDTO>()).ToList();

            return new AircraftPaginatedResponseDTO<AircraftDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                Data = aircraftDTOs
            };
        }

        public async Task<BasicResponseDTO> UpdateAircraftAsync(Guid aircraftId, AircraftDTO aircraft)
        {
            var validationResult = await _validator.ValidateAsync(aircraft);
            validationResult.ThrowIfInvalid();

            if (!await _userRepository.IsAdmin(aircraft.businessId))
            {
                throw new ZuliUnauthorizedException($"No tiene permisos para actualizar la aeronave {aircraftId}");
            }

            var existingAircraft = await _repository.GetById(aircraftId);
            if (existingAircraft == null)
            {
                throw new ZuliNotFoundException($"La aeronave con ID {aircraftId} no existe.");
            }


            aircraft.Adapt(existingAircraft, TypeAdapterConfig.GlobalSettings);

            await _repository.UpdateAircraftAsync(existingAircraft);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se actualizó la aeronave correctamente"
            };
        }
    }
}
