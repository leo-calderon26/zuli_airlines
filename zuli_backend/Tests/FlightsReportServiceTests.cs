using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Mapster;
using MapsterMapper;
using Moq;
using NUnit.Framework;
using zuli_Business;
using zuli_Business.Reports;
using zuli_Business.Interface.Reports;
using zuli_Business.Interface;
using zuli_Business.DTO.Filters;
using zuli_Business.DTO.Reports;
using zuli_Data.Entities.Filters;
using zuli_Data.Entities.Reports;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class FlightsReportServiceTests
    {
        private Mock<IFlightsReportRepository> _flightsReportRepositoryMock;
        private FlightsReportService _flightsReportService;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            Randomizer.Seed = new Random(12345);

            _flightsReportRepositoryMock = new Mock<IFlightsReportRepository>();


            var config = new TypeAdapterConfig();
            _mapper = new Mapper(config);


            _flightsReportService = new FlightsReportService(_flightsReportRepositoryMock.Object, _mapper);
        }

        [Test]
        public async Task GetFlightsReportAsync_ShouldMapDtoToEntityCorrectly_AndReturnMappedDtos()
        {

            var filterDto = new FlightsReportFilterDTO
            {
                FromDate = new DateTime(2026, 6, 1),
                ToDate = new DateTime(2026, 6, 30),
                Origin = "SJO",
                Destination = "AMS",
                FlightClass = "Economy"
            };

            var expectedEntities = BuildValidFlightsReportEntities();

            _flightsReportRepositoryMock
                .Setup(repo => repo.GetFlightsReportAsync(It.IsAny<FlightsReportFilterEntity>()))
                .ReturnsAsync(expectedEntities);


            var result = await _flightsReportService.GetFlightsReportAsync(filterDto);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(expectedEntities.Count));

            var firstResultDto = result.First();
            var firstExpectedEntity = expectedEntities.First();
            Assert.That(firstResultDto.FlightNumber, Is.EqualTo(firstExpectedEntity.FlightNumber));
            Assert.That(firstResultDto.Airline, Is.EqualTo(firstExpectedEntity.Airline));

            _flightsReportRepositoryMock.Verify(repo => repo.GetFlightsReportAsync(It.Is<FlightsReportFilterEntity>(entity =>
                entity.FromDate == filterDto.FromDate &&
                entity.ToDate == filterDto.ToDate &&
                entity.Origin == filterDto.Origin &&
                entity.Destination == filterDto.Destination &&
                entity.FlightClass == filterDto.FlightClass
            )), Times.Once);
        }

        [Test]
        public async Task GetFlightsReportAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyDtoList()
        {

            var filterDto = new FlightsReportFilterDTO { Origin = "SJO" };

            _flightsReportRepositoryMock
                .Setup(repo => repo.GetFlightsReportAsync(It.IsAny<FlightsReportFilterEntity>()))
                .ReturnsAsync(new List<FlightsReportEntity>());


            var result = await _flightsReportService.GetFlightsReportAsync(filterDto);


            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GenerateFlightsReportExcelAsync_ReturnsValidXlsxStream()
        {
            var serviceMock = new Mock<IFlightsReportService>();

            serviceMock
                .Setup(s => s.GetFlightsReportAsync(It.IsAny<FlightsReportFilterDTO>()))
                .ReturnsAsync(new List<FlightsReportDTO>
                {
            new FlightsReportDTO
            {
                Date = new DateTime(2026, 06, 22),
                Origin = "SJO",
                Destination = "FRA",
                FlightNumber = 1,
                Airline = "zuliAirline",
                FirstClassPassengers = 0,
                EconomyClassPassengers = 1,
                PassengerSales = 1000m,
                BaggageSales = 200m,
                TotalSales = 1200m
            }
                });

            var exportService = new FlightsReportExportService(serviceMock.Object);

            var stream = await exportService.GenerateFlightsReportExcelAsync(new FlightsReportFilterDTO
            {
                FromDate = DateTime.Parse("2026-06-01"),
                ToDate = DateTime.Parse("2026-06-30")
            });

            Assert.That(stream, Is.Not.Null);
            Assert.That(stream.CanRead, Is.True);

            stream.Position = 0;
            var buffer = new byte[4];
            await stream.ReadExactlyAsync(buffer, 0, 4);

            Assert.That(buffer, Is.EqualTo(new byte[] { 0x50, 0x4B, 0x03, 0x04 }));
            await stream.DisposeAsync();
        }

        private static List<FlightsReportEntity> BuildValidFlightsReportEntities()
        {
            var reportFaker = new Faker<FlightsReportEntity>()
                .RuleFor(r => r.Date, f => f.Date.Recent())
                .RuleFor(r => r.Origin, f => f.PickRandom("SJO", "AMS", "JFK"))
                .RuleFor(r => r.Destination, f => f.PickRandom("MIA", "PTY", "FRA"))
                .RuleFor(r => r.FlightNumber, f => f.Random.Int(1, 999))
                .RuleFor(r => r.FirstClassPassengers, f => f.Random.Int(0, 10))
                .RuleFor(r => r.EconomyClassPassengers, f => f.Random.Int(1, 150))
                .RuleFor(r => r.Airline, f => "zuliAirline")
                .RuleFor(r => r.PassengerSales, f => f.Random.Decimal(500, 5000))
                .RuleFor(r => r.BaggageSales, f => f.Random.Decimal(50, 500))
                .RuleFor(r => r.TotalSales, (_, currentEntity) => currentEntity.PassengerSales + currentEntity.BaggageSales);

            return reportFaker.Generate(3);
        }
    }
}