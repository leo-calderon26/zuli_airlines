using Moq;
using NUnit.Framework;
using Mapster;
using System;
using System.Threading.Tasks;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Mappings;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;
using zuli_Business.Interface;
using MapsterMapper;
using Assert = NUnit.Framework.Assert;
using Microsoft.Extensions.Configuration;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class APIServiceTests
    {
        private Mock<IFlightRepository> _flightRepositoryMock;
        private Mock<IFlightDateGenerator> _dateGeneratorMock;
        private Mock<IFlightPathFinder> _pathFinderMock;
        private Mock<IMapper> _mapperMock;

        private FlightService _flightService = null!;

        [SetUp]
        public void SetUp()
        {
            _flightRepositoryMock = new Mock<IFlightRepository>();
            _dateGeneratorMock = new Mock<IFlightDateGenerator>();
            _pathFinderMock = new Mock<IFlightPathFinder>();
            _mapperMock = new Mock<IMapper>();

            _flightService = new FlightService(
                _pathFinderMock.Object,
                _flightRepositoryMock.Object,
                _dateGeneratorMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public async Task RetrieveAvailableFlights_ValidRequest_ReturnsMappedFlights()
        {
            var request = BuildValidRequestedFlightDto();
            var rawFlights = new List<RawFlightEntity>
            {
                BuildRawFlightEntity(1, "SJO", "MEX"),
                BuildRawFlightEntity(2, "SJO", "MEX")
            };
            var scheduledFlights = rawFlights.ToList();
            var validPaths = new List<List<RawFlightEntity>>
            {
                new List<RawFlightEntity> { scheduledFlights[0] },
                new List<RawFlightEntity> { scheduledFlights[1] }
            };
            var mappedFlights = new List<BookedFlightDTO>
            {
                BuildBookedFlightDto(scheduledFlights[0].FlightGUID),
                BuildBookedFlightDto(scheduledFlights[1].FlightGUID)
            };

            _flightRepositoryMock
                .Setup(r => r.GetAvailableFlights(request.earliestDeparture, request.destination, request.passengersQuantity))
                .ReturnsAsync(rawFlights);
            _dateGeneratorMock
                .Setup(g => g.GenerateOccurrences(It.IsAny<IEnumerable<RawFlightEntity>>(), request.earliestDeparture, request.latestDeparture))
                .Returns(scheduledFlights);
            _pathFinderMock
                .Setup(p => p.FindPaths(It.Is<PathFinderParametersDTO>(criteria =>
                    criteria.Destination == request.destination &&
                    criteria.EarliestDeparture == request.earliestDeparture &&
                    criteria.LatestDeparture == request.latestDeparture &&
                    criteria.FlightPool.SequenceEqual(scheduledFlights))))
                .Returns(validPaths);
            _mapperMock
                .Setup(m => m.Map<List<BookedFlightDTO>>(It.IsAny<List<RawFlightEntity>>()))
                .Returns(mappedFlights);

            var result = await _flightService.RetrieveAvailableFlights(request);

            Assert.That(result, Is.EquivalentTo(mappedFlights));
            _flightRepositoryMock.Verify(
                r => r.GetAvailableFlights(request.earliestDeparture, request.destination, request.passengersQuantity),
                Times.Once);
            _dateGeneratorMock.Verify(
                g => g.GenerateOccurrences(It.IsAny<IEnumerable<RawFlightEntity>>(), request.earliestDeparture, request.latestDeparture),
                Times.Once);
            _pathFinderMock.Verify(
                p => p.FindPaths(It.IsAny<PathFinderParametersDTO>()),
                Times.Once);
            _mapperMock.Verify(
                m => m.Map<List<BookedFlightDTO>>(It.Is<List<RawFlightEntity>>(flights => flights.Count == 2)),
                Times.Once);
        }

        [Test]
        public void RetrieveAvailableFlights_InvalidRequest_ThrowsValidationException()
        {
            var request = new RequestedFlightDTO
            {
                destination = "A1",
                earliestDeparture = new DateTime(2026, 1, 2, 10, 0, 0),
                latestDeparture = new DateTime(2026, 1, 1, 10, 0, 0),
                passengersQuantity = 2
            };

            Assert.That(
                async () => await _flightService.RetrieveAvailableFlights(request),
                Throws.TypeOf<ZuliValidationException>());

            _flightRepositoryMock.Verify(
                r => r.GetAvailableFlights(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test]
        public async Task ValidateUser_ValidAirline_ReturnsToken()
        {
            var service = BuildAuthorizationService();
            var request = new AuthorizationDTO { airlineName = "zuli", apikey = "test-api-key-123" };

            var result = await service.ValidateUser(request);

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.CreatedToken, Is.Not.Empty);
        }


        [Test]
        public void ValidateUser_InvalidAirline_ThrowsUnauthorized()
        {
            var service = BuildAuthorizationService();
            var request = new AuthorizationDTO { airlineName = "invalidairline", apikey = "test-api-key-123" };

            Assert.That(
                async () => await service.ValidateUser(request),
                Throws.TypeOf<ZuliUnauthorizedException>());
        }

        [Test]
        public void ValidateUser_InvalidAPIKey_ThrowsUnauthorized()
        {
            var service = BuildAuthorizationService();
            var request = new AuthorizationDTO { airlineName = "zuli", apikey = "invalidkey" };

            Assert.That(
                async () => await service.ValidateUser(request),
                Throws.TypeOf<ZuliUnauthorizedException>());
        }

        [Test]
        public void ValidateUser_InvalidFormat_ThrowsValidationException()
        {
            var service = BuildAuthorizationService();
            var request = new AuthorizationDTO { airlineName = "zuli123", apikey = "test-api-key-123" };

            Assert.That(
                async () => await service.ValidateUser(request),
                Throws.TypeOf<ZuliValidationException>());
        }

        private static RequestedFlightDTO BuildValidRequestedFlightDto()
        {
            return new RequestedFlightDTO
            {
                destination = "MEX",
                earliestDeparture = new DateTime(2026, 1, 1, 8, 0, 0),
                latestDeparture = new DateTime(2026, 1, 2, 18, 0, 0),
                passengersQuantity = 2
            };
        }

        private static RawFlightEntity BuildRawFlightEntity(int routeId, string departure, string arrival)
        {
            return new RawFlightEntity
            {
                FlightGUID = Guid.NewGuid(),
                FlightRouteId = routeId,
                EstimatedDuration = 120,
                DepartureAiportCode = departure,
                DepartureAiportName = "Departure Name",
                DepartureCityName = "Departure City",
                ArrivalAiportCode = arrival,
                ArrivalAirportName = "Arrival Name",
                ArrivalAirportCity = "Arrival City",
                Frequency = 1,
                TouristPrice = 200m,
                FirstClassPrice = 300m,
                CarryOnPrice = 40m,
                CheckedPrice = 60m,
                ScheduledDepartureTime = new TimeSpan(8, 0, 0),
                ScheduledArrivalTime = new TimeSpan(10, 0, 0),
                DepartureTime = new DateTime(2026, 1, 1, 8, 0, 0),
                ArrivalTime = new DateTime(2026, 1, 1, 10, 0, 0)
            };
        }

        private static BookedFlightDTO BuildBookedFlightDto(Guid flightGuid)
        {
            return new BookedFlightDTO
            {
                flightGUID = flightGuid,
                departureTime = new DateTime(2026, 1, 1, 8, 0, 0),
                arrivalTime = new DateTime(2026, 1, 1, 10, 0, 0),
                duration = 120,
                departureAirportCode = "SJO",
                departureAirportName = "Departure Name",
                departureAirportCity = "Departure City",
                arrivalAirportCode = "MEX",
                arrivalAirportName = "Arrival Name",
                arrivalAirportCity = "Arrival City",
                touristPrice = 200m,
                firstClassPrice = 300m,
                carryOnPrice = 40m,
                checkedPrice = 60m
            };
        }

        private static AuthorizationService BuildAuthorizationService()
        {
            var settings = new Dictionary<string, string>
            {
                {"settings:secretKey", "test-secret-key-123"},
                {"settings:apiKey", "test-api-key-123" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            return new AuthorizationService(configuration);
        }
    }
}