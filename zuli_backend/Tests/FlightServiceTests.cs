using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class FlightServiceTests
    {
        private Mock<IFlightRepository> _flightRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IServiceRepository> _serviceRepoMock;
        private Mock<IFlightPathFinder> _pathFinderMock;
        private Mock<IValidator<FlightDTO>> _flightValidatorMock;
        private Mock<IValidator<FlightSearchRequestDTO>> _searchValidatorMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<FlightAvailabilityRequestDTO>> _availabilityValidatorMock;
        
        private FlightService _flightService;

        [SetUp]
        public void SetUp()
        {
            _flightRepoMock = new Mock<IFlightRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _serviceRepoMock = new Mock<IServiceRepository>();
            _pathFinderMock = new Mock<IFlightPathFinder>();
            _flightValidatorMock = new Mock<IValidator<FlightDTO>>();
            _searchValidatorMock = new Mock<IValidator<FlightSearchRequestDTO>>();
            _mapperMock = new Mock<IMapper>();
            _availabilityValidatorMock = new Mock<IValidator<FlightAvailabilityRequestDTO>>();

            _flightService = new FlightService(
                _flightRepoMock.Object,
                _userRepoMock.Object,
                _serviceRepoMock.Object,
                _pathFinderMock.Object,
                _flightValidatorMock.Object,
                _searchValidatorMock.Object,
                _availabilityValidatorMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task CreateFlight_WithValidData_ReturnsSuccess()
        {
            var validFlightRequestWithoutService = new FlightDTO 
            { 
                BusinessId = "123456789", 
                ServiceDescription = string.Empty 
            };
            
            var successfulValidationResult = new ValidationResult(); 
            var expectedAdminUserId = Guid.NewGuid();
            var mockedFlightEntity = new FlightEntity();

            _flightValidatorMock
                .Setup(validator => validator.ValidateAsync(validFlightRequestWithoutService, It.IsAny<CancellationToken>()))
                .ReturnsAsync(successfulValidationResult);

            _userRepoMock
                .Setup(repo => repo.GetUserId(validFlightRequestWithoutService.BusinessId))
                .ReturnsAsync(expectedAdminUserId);

            _mapperMock
                .Setup(mapper => mapper.Map<FlightEntity>(validFlightRequestWithoutService))
                .Returns(mockedFlightEntity);


            var operationResponse = await _flightService.CreateFlight(validFlightRequestWithoutService);

            Assert.That(operationResponse, Is.Not.Null);
            Assert.That(operationResponse.StatusCode, Is.EqualTo(200));
            Assert.That(operationResponse.Message, Is.EqualTo("Se realizo la creacion del vuelo correctamente"));
            
            _serviceRepoMock.Verify(repo => repo.CreateService(It.IsAny<ServiceEntity>()), Times.Never);
        }

        [Test]
        public async Task CreateFlight_WithValidDataAndService_ReturnsSuccess()
        {
            var validFlightRequestWithService = new FlightDTO 
            { 
                BusinessId = "123456789", 
                ServiceDescription = "   Servicio VIP   " 
            };
            
            var successfulValidationResult = new ValidationResult();
            var expectedAdminUserId = Guid.NewGuid();
            var mockedFlightEntity = new FlightEntity();

            _flightValidatorMock
                .Setup(validator => validator.ValidateAsync(validFlightRequestWithService, It.IsAny<CancellationToken>()))
                .ReturnsAsync(successfulValidationResult);

            _userRepoMock
                .Setup(repo => repo.GetUserId(validFlightRequestWithService.BusinessId))
                .ReturnsAsync(expectedAdminUserId);

            _mapperMock
                .Setup(mapper => mapper.Map<FlightEntity>(validFlightRequestWithService))
                .Returns(mockedFlightEntity);

            var operationResponse = await _flightService.CreateFlight(validFlightRequestWithService);

            Assert.That(operationResponse, Is.Not.Null);
            Assert.That(operationResponse.StatusCode, Is.EqualTo(200));
            Assert.That(operationResponse.Message, Is.EqualTo("Se realizo la creacion del vuelo correctamente"));
            
            _serviceRepoMock.Verify(repo => repo.CreateService(It.Is<ServiceEntity>(entity => 
                entity.FlightId == mockedFlightEntity.Id && 
                entity.Description == "Servicio VIP"
            )), Times.Once);
        }

        [Test]
        public void CreateFlight_WithInvalidData_ThrowsZuliValidationException()
        {
            var invalidFlightRequest = new FlightDTO 
            { 
                BusinessId = string.Empty 
            };
            
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("BusinessId", "Debe especificar un administrador valido")
            };
            var failedValidationResult = new ValidationResult(validationErrors);

            _flightValidatorMock
                .Setup(validator => validator.ValidateAsync(invalidFlightRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedValidationResult);

            var thrownException = Assert.ThrowsAsync<ZuliValidationException>(
                async () => await _flightService.CreateFlight(invalidFlightRequest)
            );

            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.Errors.ContainsKey("BusinessId"), Is.True);
            
            _userRepoMock.Verify(repo => repo.GetUserId(It.IsAny<string>()), Times.Never);
            _mapperMock.Verify(mapper => mapper.Map<FlightEntity>(It.IsAny<FlightDTO>()), Times.Never);
            _serviceRepoMock.Verify(repo => repo.CreateService(It.IsAny<ServiceEntity>()), Times.Never);
        }

        [Test]
        public async Task GetAllFlights_WhenNoFlightsExist_ReturnsEmptyList()
        {
            var emptyFlightEntitiesList = new List<FlightEntity>();
            var emptyFlightDtosList = new List<FlightDTO>();

            _flightRepoMock
                .Setup(repo => repo.GetAllFlights())
                .ReturnsAsync(emptyFlightEntitiesList);

            _mapperMock
                .Setup(mapper => mapper.Map<List<FlightDTO>>(emptyFlightEntitiesList))
                .Returns(emptyFlightDtosList);

            var operationResponse = await _flightService.GetAllFlights();

            Assert.That(operationResponse, Is.Not.Null);
            Assert.That(operationResponse, Is.Empty);

            _userRepoMock.Verify(repo => repo.GetBusinessId(It.IsAny<Guid>()), Times.Never);
            _serviceRepoMock.Verify(repo => repo.GetServiceByFlightId(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task GetAllFlights_WhenFlightsExist_MapsDataCorrectly()
        {
            var firstFlightId = Guid.NewGuid();
            var secondFlightId = Guid.NewGuid();
            
            var firstAdminId = Guid.NewGuid();
            var secondAdminId = Guid.NewGuid();

            var expectedFirstBusinessId = "BUSINESS-001";
            var expectedSecondBusinessId = "BUSINESS-002";

            var mockedFlightEntities = new List<FlightEntity>
            {
                new FlightEntity { Id = firstFlightId, AdminId = firstAdminId },
                new FlightEntity { Id = secondFlightId, AdminId = secondAdminId }
            };

            var mappedFlightDtos = new List<FlightDTO>
            {
                new FlightDTO { Id = firstFlightId },
                new FlightDTO { Id = secondFlightId }
            };

            var mockedServiceForFirstFlight = new ServiceEntity { Description = "Almuerzo VIP incluido" };

            _flightRepoMock
                .Setup(repo => repo.GetAllFlights())
                .ReturnsAsync(mockedFlightEntities);

            _userRepoMock
                .Setup(repo => repo.GetBusinessId(firstAdminId))
                .ReturnsAsync(expectedFirstBusinessId);

            _userRepoMock
                .Setup(repo => repo.GetBusinessId(secondAdminId))
                .ReturnsAsync(expectedSecondBusinessId);

            _serviceRepoMock
                .Setup(repo => repo.GetServiceByFlightId(firstFlightId))
                .ReturnsAsync(mockedServiceForFirstFlight);

            _serviceRepoMock
                .Setup(repo => repo.GetServiceByFlightId(secondFlightId))
                .ReturnsAsync((ServiceEntity)null);

            _mapperMock
                .Setup(mapper => mapper.Map<List<FlightDTO>>(mockedFlightEntities))
                .Returns(mappedFlightDtos);

            var operationResponse = (List<FlightDTO>)await _flightService.GetAllFlights();

            Assert.That(operationResponse, Is.Not.Null);
            Assert.That(operationResponse.Count, Is.EqualTo(2));


            var firstResultDto = operationResponse[0];
            Assert.That(firstResultDto.Id, Is.EqualTo(firstFlightId));
            Assert.That(firstResultDto.BusinessId, Is.EqualTo(expectedFirstBusinessId));
            Assert.That(firstResultDto.ServiceDescription, Is.EqualTo("Almuerzo VIP incluido"));

            var secondResultDto = operationResponse[1];
            Assert.That(secondResultDto.Id, Is.EqualTo(secondFlightId));
            Assert.That(secondResultDto.BusinessId, Is.EqualTo(expectedSecondBusinessId));
            Assert.That(secondResultDto.ServiceDescription, Is.Null);

            _userRepoMock.Verify(repo => repo.GetBusinessId(It.IsAny<Guid>()), Times.Exactly(2));
            _serviceRepoMock.Verify(repo => repo.GetServiceByFlightId(It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Test]
        public void Search_InvalidRequest_ThrowsZuliValidationException()
        {
            var request = new FlightSearchRequestDTO();
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Origin", "El origen es requerido")
            };

            _searchValidatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            Assert.That(async () => await _flightService.Search(request), Throws.TypeOf<ZuliValidationException>());

            _flightRepoMock.Verify(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _pathFinderMock.Verify(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>()), Times.Never);
        }


        [Test]
        public async Task Search_OneWayFlight_ReturnsDepartureFlightsOnly()
        {
            var request = BuildValidRequest(isRoundTrip: false);
            SetupMocksForSuccessfulSearch();

            var result = await _flightService.Search(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecordsDeparture, Is.EqualTo(1));
            Assert.That(result.DepartureFlights.Count, Is.EqualTo(1));

            Assert.That(result.TotalRecordsReturn, Is.EqualTo(0));
            Assert.That(result.ReturnFlights, Is.Empty);

            _flightRepoMock.Verify(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
            _pathFinderMock.Verify(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>()), Times.Once);
        }

        [Test]
        public async Task Search_NoResults_ReturnsEmptyListWithoutErrors()
        {
            var request = BuildValidRequest(isRoundTrip: false);

            _searchValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _flightRepoMock.Setup(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<RawFlightEntity>());

            _pathFinderMock.Setup(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>())).Returns(new List<List<RawFlightEntity>>());
            _mapperMock.Setup(m => m.Map<List<FlightSearchResponseDTO>>(It.IsAny<List<List<RawFlightEntity>>>())).Returns(new List<FlightSearchResponseDTO>());

            var result = await _flightService.Search(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecordsDeparture, Is.EqualTo(0));
            Assert.That(result.DepartureFlights, Is.Empty);
        }

        [Test]
        public async Task Search_RoundTrip_ReturnsDepartureAndReturnFlights()
        {
            var request = BuildValidRequest(isRoundTrip: true, returnDate: new DateTime(2026, 6, 20));
            SetupMocksForSuccessfulSearch();

            var result = await _flightService.Search(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecordsDeparture, Is.EqualTo(1), "Debe traer registros de ida");
            Assert.That(result.DepartureFlights.Count, Is.EqualTo(1));

            Assert.That(result.TotalRecordsReturn, Is.EqualTo(1), "Debe traer registros de vuelta");
            Assert.That(result.ReturnFlights.Count, Is.EqualTo(1));

            _flightRepoMock.Verify(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(4));
            _pathFinderMock.Verify(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>()), Times.Exactly(2));
        }

        [Test]
        public async Task Search_RoundTrip_WithoutReturnDate_IgnoresReturnFlight()
        {
            var request = BuildValidRequest(isRoundTrip: true, returnDate: null);
            SetupMocksForSuccessfulSearch();

            var result = await _flightService.Search(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecordsDeparture, Is.EqualTo(1));
            Assert.That(result.TotalRecordsReturn, Is.EqualTo(0), "Al ser nula la fecha de regreso, no debe buscar retorno");
            Assert.That(result.ReturnFlights, Is.Empty);

            _flightRepoMock.Verify(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
            _pathFinderMock.Verify(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>()), Times.Once);
        }

        [Test]
        public async Task Search_Pagination_CalculatesCorrectly()
        {
            var request = BuildValidRequest(isRoundTrip: false);
            request.Page = 2;
            request.PageSize = 10;

            _searchValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _flightRepoMock.Setup(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<RawFlightEntity>());
            _pathFinderMock.Setup(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>())).Returns(new List<List<RawFlightEntity>>());

            var mockOptions = Enumerable.Range(1, 15).Select(i => new FlightSearchResponseDTO
            {
                TotalTouristPrice = i * 10,
                Stops = 0
            }).ToList();

            _mapperMock.Setup(m => m.Map<List<FlightSearchResponseDTO>>(It.IsAny<List<List<RawFlightEntity>>>())).Returns(mockOptions);

            var result = await _flightService.Search(request);

            Assert.That(result.TotalRecordsDeparture, Is.EqualTo(15), "El total de registros debe ser 15");
            Assert.That(result.TotalPagesDeparture, Is.EqualTo(2), "El total de páginas debe ser 2");
            Assert.That(result.DepartureFlights.Count, Is.EqualTo(5), "La página 2 debe tener 5 registros (15 - 10)");

            Assert.That(result.DepartureFlights.First().TotalTouristPrice, Is.EqualTo(110));
        }

        [Test]
        public async Task Search_Sorting_SortsByPriceAscendingAndStopsAscending()
        {
            var request = BuildValidRequest(isRoundTrip: false);

            _searchValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _flightRepoMock.Setup(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<RawFlightEntity>());
            _pathFinderMock.Setup(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>())).Returns(new List<List<RawFlightEntity>>());

            var mockOptions = new List<FlightSearchResponseDTO>
            {
                new FlightSearchResponseDTO { TotalTouristPrice = 300, Stops = 1 },
                new FlightSearchResponseDTO { TotalTouristPrice = 100, Stops = 2 },
                new FlightSearchResponseDTO { TotalTouristPrice = 100, Stops = 0 },
            };

            _mapperMock.Setup(m => m.Map<List<FlightSearchResponseDTO>>(It.IsAny<List<List<RawFlightEntity>>>())).Returns(mockOptions);

            var result = await _flightService.Search(request);
            var sortedFlights = result.DepartureFlights;

            Assert.That(sortedFlights.Count, Is.EqualTo(3));

            Assert.That(sortedFlights[0].TotalTouristPrice, Is.EqualTo(100));
            Assert.That(sortedFlights[0].Stops, Is.EqualTo(0));

            Assert.That(sortedFlights[1].TotalTouristPrice, Is.EqualTo(100));
            Assert.That(sortedFlights[1].Stops, Is.EqualTo(2));

            Assert.That(sortedFlights[2].TotalTouristPrice, Is.EqualTo(300));
            Assert.That(sortedFlights[2].Stops, Is.EqualTo(1));
        }

        private FlightSearchRequestDTO BuildValidRequest(bool isRoundTrip, DateTime? returnDate = null)
        {
            return new FlightSearchRequestDTO
            {
                Origin = "SJO",
                Destination = "MIA",
                Date = new DateTime(2026, 6, 15),
                Seats = 1,
                IsRoundTrip = isRoundTrip,
                ReturnDate = returnDate,
                FlightClass = "Turista",
                Page = 1,
                PageSize = 10
            };
        }

        private void SetupMocksForSuccessfulSearch()
        {
            _searchValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<FlightSearchRequestDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _flightRepoMock
                .Setup(r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<RawFlightEntity>());

            var mockPaths = new List<List<RawFlightEntity>> { new List<RawFlightEntity>() };
            _pathFinderMock
                .Setup(p => p.FindPaths(It.IsAny<PathFinderParametersDTO>()))
                .Returns(mockPaths);

            var mockOptions = new List<FlightSearchResponseDTO>
            {
                new FlightSearchResponseDTO { TotalTouristPrice = 250, Stops = 0 }
            };
            _mapperMock
                .Setup(m => m.Map<List<FlightSearchResponseDTO>>(It.IsAny<List<List<RawFlightEntity>>>()))
                .Returns(mockOptions);
        }

    }
}