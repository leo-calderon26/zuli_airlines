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
        private readonly IUserRepository _userRepository;
        private readonly AirportValidator _validator;

        public AirportService(IAirportRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _validator = new AirportValidator();
        }

        public async Task<BasicResponseDTO> CreateAirport(AirportDTO airport)
        {

            if (!await _userRepository.IsAdmin(airport.businessId))
            {
                throw new ZuliNotFoundException($"El Usuario que esta intentando crear un aeropuerto y no tiene permisos {airport.businessId}");
            }
            if (await _repository.AlreadyExist(airport.airportCode))
            {
                throw new ZuliNotFoundException($"Se encontro un aeropuerto con el mismo codigo {airport.airportCode}");
            }

            var userId = await _userRepository.GetUserId(airport.businessId);
            _validator.ValidateAirportInfo(airport);

            var newAirport = new AirportEntity
            {
                AirportCode = airport.airportCode,
                Name = airport.name,
                Country = airport.country,
                City = airport.city,
                AdminId = userId,
            };

            await _repository.CreateAirport(newAirport);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion del aeropuerto correctamente",
            };
        }

        public async Task<IEnumerable<AirportDTO>> GetAll()
        {
            var airports = await _repository.GetAll();

            return airports.Select(item => new AirportDTO
            {
                airportCode = item.AirportCode,
                name = item.Name,
                country = item.Country,
                city = item.City,
                businessId = item.AdminId.ToString()
            }).ToList();
        }

        public async Task<List<AirportSuggestionDTO>> GetAirportSuggestions(string searchTerm)
        {
            _validator.ValidateSearchTerm(searchTerm);

            var airports = await _repository.SearchAirportsByTerm(searchTerm.Trim());

            var suggestions = airports.Select(a => new AirportSuggestionDTO
            {
                AirportCode = a.AirportCode,
                DisplayName = $"{a.AirportCode} - {a.City}, {a.Country} ({a.Name})"
            }).ToList();

            return suggestions;
        }
    }
}