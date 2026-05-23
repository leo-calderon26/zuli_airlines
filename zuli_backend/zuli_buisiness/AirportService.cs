using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
                throw new ZuliValidationException(AirportAtributes.CODE, $"Se encontro un aeropuerto con el mismo codigo {airport.airportCode}");
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

        public async Task<AirportPaginatedResponseDTO> GetAirportsPaginated(int pageNumber, int pageSize)
        {
            var (airports, totalCount) = await _repository.GetAirportsPaginated(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var airportDTOs = airports.Select(item => new AirportDTO
            {
                airportCode = item.AirportCode,
                name = item.Name,
                country = item.Country,
                city = item.City,
                businessId = item.AdminId.ToString()
            }).ToList();

            return new AirportPaginatedResponseDTO
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                Data = airportDTOs
            };
        }

        public async Task<AirportDTO> GetAirportByCodeAsync(string code)
        {
            var normalizedCode = code?.Trim().ToUpperInvariant();
            var entity = await _repository.GetByCodeAsync(normalizedCode);
            if (entity == null) throw new ZuliNotFoundException($"No existe aeropuerto {code}");
            return new AirportDTO { airportCode = entity.AirportCode, name = entity.Name, country = entity.Country, city = entity.City, businessId = entity.AdminId.ToString() };
        }

        public async Task<BasicResponseDTO> UpdateAirportAsync(string code, AirportDTO airport)
        {
            if (!await _userRepository.IsAdmin(airport.businessId))
                throw new ZuliUnauthorizedException("No tiene permisos.");

            var normalizedCode = code?.Trim().ToUpperInvariant();
            var existing = await _repository.GetByCodeAsync(normalizedCode);
            if (existing == null) throw new ZuliNotFoundException($"No existe aeropuerto {code}");

            _validator.ValidateAirportInfo(airport);

            existing.Name = airport.name;
            existing.Country = airport.country;
            existing.City = airport.city;

            await _repository.UpdateAirportAsync(existing);

            return new BasicResponseDTO { StatusCode = 200, Message = "Aeropuerto actualizado correctamente" };
        }


    }
}