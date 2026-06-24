using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Mapster;       // 🚀 Nuevo: Necesario para la configuración de Mapster
using MapsterMapper; // 🚀 Nuevo: Necesario para usar la interfaz IMapper
using Moq;
using NUnit.Framework;
using zuli_Business;
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
            Assert.That(firstResultDto.NumeroVuelo, Is.EqualTo(firstExpectedEntity.NumeroVuelo));
            Assert.That(firstResultDto.Aerolinea, Is.EqualTo(firstExpectedEntity.Aerolinea));

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

        private static List<FlightsReportEntity> BuildValidFlightsReportEntities()
        {
            var reportFaker = new Faker<FlightsReportEntity>()
                .RuleFor(r => r.Fecha, f => f.Date.Recent())
                .RuleFor(r => r.Origen, f => f.PickRandom("SJO", "AMS", "JFK"))
                .RuleFor(r => r.Destino, f => f.PickRandom("MIA", "PTY", "FRA"))
                .RuleFor(r => r.NumeroVuelo, f => f.Random.Int(1, 999))
                .RuleFor(r => r.PasajerosPrimera, f => f.Random.Int(0, 10))
                .RuleFor(r => r.PasajerosEconomica, f => f.Random.Int(1, 150))
                .RuleFor(r => r.Aerolinea, f => "zuliAirline")
                .RuleFor(r => r.VentaPasajeros, f => f.Random.Decimal(500, 5000))
                .RuleFor(r => r.VentaEquipaje, f => f.Random.Decimal(50, 500))
                .RuleFor(r => r.TotalVenta, (_, currentEntity) => currentEntity.VentaPasajeros + currentEntity.VentaEquipaje);

            return reportFaker.Generate(3);
        }
    }
}