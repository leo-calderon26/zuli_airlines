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

            _flightService = new FlightService(
                _flightRepoMock.Object,
                _userRepoMock.Object,
                _serviceRepoMock.Object,
                _pathFinderMock.Object,
                _flightValidatorMock.Object,
                _searchValidatorMock.Object,
                _mapperMock.Object
            );
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