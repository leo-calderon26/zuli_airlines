using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.Extensions.Time.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using zuli_Business;
using zuli_Business.DTO.ReservationSearch;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;
using zuli_Business.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class ReservationSearchServiceTests
    {
        private const string ValidReservationCode = "ABC12345";
        private const string LowerCaseReservationCode = "abc12345";
        private const string ValidLastName = "Gomez";
        private const string InvalidReservationCode = "PAPUA";
        private const string InvalidLastName = "";
        private const string DestinationCity = "Ciudad de Panamá";
        private const string DestinationCode = "PTY";

        private const int ExpectedDaysRemaining = 10;
        private const int ExpectedSegmentCount = 1;

        // Fecha de simulación para las pruebas, utilizando el huso horario de Costa Rica (UTC-6).
        private readonly DateTimeOffset TestCurrentDate =
            new DateTimeOffset(2026, 1, 1, 12, 0, 0, new TimeSpan(-6, 0, 0));

        private readonly DateTime TestDepartureDate =
            new DateTime(2026, 1, 11, 12, 0, 0);

        private readonly DateTime TestArrivalDate =
            new DateTime(2026, 1, 11, 14, 0, 0);

        private Mock<IReservationSearchRepository> _repositoryMock;
        private Mock<IValidator<ReservationSearchRequestDTO>> _validatorMock;
        private Mock<IMapper> _mapperMock;
        
        private FakeTimeProvider _fakeTimeProvider;
        private Mock<IReservationItineraryPdfService> _pdfServiceMock;

        private ReservationSearchService _service;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IReservationSearchRepository>();
            _validatorMock = new Mock<IValidator<ReservationSearchRequestDTO>>();
            _mapperMock = new Mock<IMapper>();
            _pdfServiceMock = new Mock<IReservationItineraryPdfService>();

            _fakeTimeProvider = new CostaRicaTimeProvider();
            _fakeTimeProvider.SetUtcNow(TestCurrentDate);

            _service = new ReservationSearchService(
                _repositoryMock.Object,
                _validatorMock.Object,
                _mapperMock.Object,
                _fakeTimeProvider,
                _pdfServiceMock?.Object
            );
        }

        [Test]
        public async Task GetReservationDetailsAsync_ValidRequestAndDataFound_ReturnsResponse()
        {
            var request = new ReservationSearchRequestDTO
            {
                ReservationCode = LowerCaseReservationCode,
                LastName = ValidLastName
            };

            var flightsEntityList = new List<ReservationSearchFlightEntity>
            {
                new ReservationSearchFlightEntity
                {
                    DepartureDateTime = TestDepartureDate,
                    ArrivalDateTime = TestArrivalDate,
                    DestinationCity = DestinationCity,
                    DestinationCode = DestinationCode
                }
            };

            var passengersEntityList = new List<ReservationSearchPassengerEntity>
            {
                new ReservationSearchPassengerEntity { FirstName = "Luis", FirstLastName = "Gomez", SecondLastName = "Oses" },
                new ReservationSearchPassengerEntity { FirstName = "Ana", FirstLastName = "Gomez", SecondLastName = "Mora" }
            };

            var expectedMappedResponse = new ReservationSearchResponseDTO
            {
                Journey = new ReservationSearchJourneyDTO
                {
                    Segments = new List<ReservationSearchSegmentDTO> { new ReservationSearchSegmentDTO() }
                }
            };

            var expectedMappedPassengers = new List<ReservationSearchPassengerDTO>
            {
                new ReservationSearchPassengerDTO { FirstName = "Luis", FirstLastName = "Gomez", SecondLastName = "Oses" },
                new ReservationSearchPassengerDTO { FirstName = "Ana", FirstLastName = "Gomez", SecondLastName = "Mora" }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock.Setup(r => r.GetReservationDataAsync(request.ReservationCode, It.IsAny<string>()))
                .ReturnsAsync((flightsEntityList, passengersEntityList));

            _mapperMock.Setup(m => m.Map<ReservationSearchResponseDTO>(flightsEntityList))
                .Returns(expectedMappedResponse);

            _mapperMock.Setup(m => m.Map<List<ReservationSearchPassengerDTO>>(passengersEntityList))
                .Returns(expectedMappedPassengers);

            var result = await _service.GetReservationDetailsAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReservationCode, Is.EqualTo(request.ReservationCode));
            Assert.That(result.PassengerCount, Is.EqualTo(passengersEntityList.Count));
            Assert.That(result.Passengers, Is.Not.Null);
            Assert.That(result.Passengers.Count, Is.EqualTo(passengersEntityList.Count));
            Assert.That(result.DaysRemaining, Is.EqualTo(ExpectedDaysRemaining));
            Assert.That(result.Journey.Segments.Count, Is.EqualTo(ExpectedSegmentCount));

            _repositoryMock.Verify(
                r => r.GetReservationDataAsync(request.ReservationCode, It.IsAny<string>()),
                Times.Once
            );
        }

        [Test]
        public void GetReservationDetailsAsync_NoDataFound_ThrowsZuliNotFoundException()
        {
            var request = new ReservationSearchRequestDTO
            {
                ReservationCode = ValidReservationCode,
                LastName = ValidLastName
            };

            var emptyFlightList = new List<ReservationSearchFlightEntity>();
            var emptyPassengersList = new List<ReservationSearchPassengerEntity>();

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock.Setup(r => r.GetReservationDataAsync(request.ReservationCode, It.IsAny<string>()))
                .ReturnsAsync((emptyFlightList, emptyPassengersList));

            Assert.That(
                async () => await _service.GetReservationDetailsAsync(request),
                Throws.TypeOf<ZuliNotFoundException>()
            );
        }

        [Test]
        public void GetReservationDetailsAsync_InvalidRequest_ThrowsValidationException()
        {
            var request = new ReservationSearchRequestDTO
            {
                ReservationCode = InvalidReservationCode,
                LastName = InvalidLastName
            };

            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure(nameof(ReservationSearchRequestDTO.ReservationCode), "Error")
            };

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            Assert.That(
                async () => await _service.GetReservationDetailsAsync(request),
                Throws.TypeOf<ZuliValidationException>()
            );

            _repositoryMock.Verify(
                r => r.GetReservationDataAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Test]
        public async Task GenerateItineraryPdfAsync_ValidRequestAndDataFound_ReturnsFileTuple()
        {
            // Arrange
            var request = new ReservationSearchRequestDTO
            {
                ReservationCode = LowerCaseReservationCode,
                LastName = ValidLastName
            };

            var flightsEntityList = new List<ReservationSearchFlightEntity>
            {
                new ReservationSearchFlightEntity
                {
                    DepartureDateTime = TestDepartureDate,
                    ArrivalDateTime = TestArrivalDate,
                    DestinationCity = DestinationCity,
                    DestinationCode = DestinationCode,
                    ReservationStatusId = 1
                }
            };
            var passengersEntityList = new List<ReservationSearchPassengerEntity>();

            var expectedResponseDto = new ReservationSearchResponseDTO { ReservationCode = LowerCaseReservationCode };
            var expectedPdfBytes = new byte[] { 1, 2, 3, 4, 5 };

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock.Setup(r => r.GetReservationDataAsync(request.ReservationCode, It.IsAny<string>()))
                .ReturnsAsync((flightsEntityList, passengersEntityList));

            _mapperMock.Setup(m => m.Map<ReservationSearchResponseDTO>(It.IsAny<List<ReservationSearchFlightEntity>>()))
                .Returns(expectedResponseDto);
                
            _mapperMock.Setup(m => m.Map<List<ReservationSearchPassengerDTO>>(It.IsAny<List<ReservationSearchPassengerEntity>>()))
                .Returns([]);

            _pdfServiceMock.Setup(p => p.GenerateItineraryPdf(It.IsAny<ReservationSearchResponseDTO>()))
                .Returns(expectedPdfBytes);

            var result = await _service.GenerateItineraryPdfAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.FileContents, Is.EqualTo(expectedPdfBytes));
                Assert.That(result.ContentType, Is.EqualTo("application/pdf"));
                Assert.That(result.FileName, Is.EqualTo($"Itinerario_{LowerCaseReservationCode}.pdf"));
            });
            
            _pdfServiceMock.Verify(p => p.GenerateItineraryPdf(It.IsAny<ReservationSearchResponseDTO>()), Times.Once);

        }
    }
    public class CostaRicaTimeProvider : FakeTimeProvider
    {
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.FindSystemTimeZoneById("America/Costa_Rica");
    }
}