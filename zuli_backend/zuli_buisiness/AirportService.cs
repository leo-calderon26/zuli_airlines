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
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _repository;
        private readonly AirportValidator _validator;

        public AirportService(IAirportRepository repository)
        {
            _repository = repository;
            _validator = new AirportValidator();
        }

        public async Task<BasicResponseDTO> CreateAirport(AirportDTO airport)
        {
            if (await _repository.AlreadyExist(airport.airportCode))
            {
                throw new ZuliNotFoundException($"Se encontro un aeropuerto con el mismo codigo {airport.airportCode}");
            }

            _validator.ValidateAirportInfo(airport);

            var newAirport = new AirportEntity
            {
                AirportCode = airport.airportCode,
                Name = airport.name,
                Country = airport.country,
                City = airport.city,
                AdminId = airport.adminId
            };

            await _repository.CreateAirport(newAirport);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion del aeropuerto correctamente",
            };
        }
    }
}