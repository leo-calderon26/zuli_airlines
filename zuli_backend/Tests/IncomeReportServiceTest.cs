using Bogus;
using MapsterMapper;
using Moq;
using zuli_Business.DTO.Reports;
using zuli_Business.Reports;
using zuli_Data.Entities.Reports;
using zuli_Repository.Interface.Reports;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class IncomeReportServiceTests
    {
        private Mock<IIncomeReportRepository> _incomeReportRepositoryMock;
        private Mock<IMapper> _mapperMock;

        private IncomeReportService _incomeReportService;

        [SetUp]
        public void SetUp()
        {
            Randomizer.Seed = new Random(12345);

            _incomeReportRepositoryMock = new Mock<IIncomeReportRepository>();
            _mapperMock = new Mock<IMapper>();

            _incomeReportService = new IncomeReportService(
                _incomeReportRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetIncomeReportAsync_ReturnsMappedDto()
        {
            var requestDto = BuildValidIncomeReportRequestDto();
            var requestEntity = BuildValidIncomeReportRequestEntity();
            var resultEntity = BuildValidIncomeReportResultEntity();
            var expectedDto = BuildValidIncomeReportResultDto();

            _mapperMock
                .Setup(mapper => mapper.Map<IncomeReportRequestEntity>(requestDto))
                .Returns(requestEntity);

            _incomeReportRepositoryMock
                .Setup(repository => repository.GetIncomeReportAsync(requestEntity))
                .ReturnsAsync(resultEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<IncomeReportResultDTO>(resultEntity))
                .Returns(expectedDto);

            var result = await _incomeReportService.GetIncomeReportAsync(requestDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expectedDto));

            _incomeReportRepositoryMock.Verify(
                repository => repository.GetIncomeReportAsync(requestEntity),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<IncomeReportRequestEntity>(requestDto),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<IncomeReportResultDTO>(resultEntity),
                Times.Once
            );
        }

        [Test]
        public async Task GetIncomeReportAsync_WithEmptyRows_ReturnsMappedDtoWithEmptyLists()
        {
            var requestDto = BuildValidIncomeReportRequestDto();
            var requestEntity = BuildValidIncomeReportRequestEntity();

            var resultEntity = new IncomeReportResultEntity
            {
                Rows = new List<IncomeReportRowEntity>(),
                Summary = new IncomeReportSummaryEntity()
            };

            var expectedDto = new IncomeReportResultDTO
            {
                Rows = new List<IncomeReportRowDTO>(),
                Summary = new IncomeReportSummaryDTO()
            };

            _mapperMock
                .Setup(mapper => mapper.Map<IncomeReportRequestEntity>(requestDto))
                .Returns(requestEntity);

            _incomeReportRepositoryMock
                .Setup(repository => repository.GetIncomeReportAsync(requestEntity))
                .ReturnsAsync(resultEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<IncomeReportResultDTO>(resultEntity))
                .Returns(expectedDto);

            var result = await _incomeReportService.GetIncomeReportAsync(requestDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Rows, Is.Empty);
            Assert.That(result.Summary, Is.Not.Null);
        }

        private static IncomeReportRequestDTO BuildValidIncomeReportRequestDto()
        {
            return new Faker<IncomeReportRequestDTO>()
                .RuleFor(r => r.Year, f => f.Random.Int(2020, 2030))
                .RuleFor(r => r.Origin, f => f.PickRandom("SJO", "MIA", "PTY", "LAX", "MAD", "JFK"))
                .RuleFor(r => r.Destination, f => f.PickRandom("SJO", "MIA", "PTY", "LAX", "MAD", "JFK"))
                .RuleFor(r => r.AirlineId, f => f.Random.Int(1, 100))
                .Generate();
        }

        private static IncomeReportRequestEntity BuildValidIncomeReportRequestEntity()
        {
            return new Faker<IncomeReportRequestEntity>()
                .RuleFor(r => r.Year, f => f.Random.Int(2020, 2030))
                .RuleFor(r => r.Origin, f => f.PickRandom("SJO", "MIA", "PTY", "LAX", "MAD", "JFK"))
                .RuleFor(r => r.Destination, f => f.PickRandom("SJO", "MIA", "PTY", "LAX", "MAD", "JFK"))
                .RuleFor(r => r.AirlineId, f => f.Random.Int(1, 100))
                .Generate();
        }

        private static IncomeReportResultEntity BuildValidIncomeReportResultEntity()
        {
            var rowFaker = new Faker<IncomeReportRowEntity>()
                .RuleFor(r => r.Year, f => f.Random.Int(2020, 2030))
                .RuleFor(r => r.Month, f => f.Random.Int(1, 12))
                .RuleFor(r => r.Flights, f => f.Random.Int(1, 100))
                .RuleFor(r => r.FirstClass, f => f.Random.Int(0, 50))
                .RuleFor(r => r.TouristClass, f => f.Random.Int(0, 200))
                .RuleFor(r => r.TotalPassengers, f => f.Random.Int(0, 250))
                .RuleFor(r => r.TicketIncome, f => f.Random.Decimal(1000, 50000))
                .RuleFor(r => r.BaggageIncome, f => f.Random.Decimal(100, 10000))
                .RuleFor(r => r.TotalIncome, f => f.Random.Decimal(1100, 60000));

            var rows = rowFaker.Generate(12);

            return new IncomeReportResultEntity
            {
                Rows = rows,
                Summary = new IncomeReportSummaryEntity
                {
                    TotalFlights = rows.Sum(r => r.Flights),
                    TotalFirstClass = rows.Sum(r => r.FirstClass),
                    TotalTouristClass = rows.Sum(r => r.TouristClass),
                    TotalPassengers = rows.Sum(r => r.TotalPassengers),
                    TotalTicketIncome = rows.Sum(r => r.TicketIncome),
                    TotalBaggageIncome = rows.Sum(r => r.BaggageIncome),
                    TotalIncome = rows.Sum(r => r.TotalIncome)
                }
            };
        }

        private static IncomeReportResultDTO BuildValidIncomeReportResultDto()
        {
            var rowFaker = new Faker<IncomeReportRowDTO>()
                .RuleFor(r => r.Year, f => f.Random.Int(2020, 2030))
                .RuleFor(r => r.Month, f => f.Random.Int(1, 12))
                .RuleFor(r => r.Flights, f => f.Random.Int(1, 100))
                .RuleFor(r => r.FirstClass, f => f.Random.Int(0, 50))
                .RuleFor(r => r.TouristClass, f => f.Random.Int(0, 200))
                .RuleFor(r => r.TotalPassengers, f => f.Random.Int(0, 250))
                .RuleFor(r => r.TicketIncome, f => f.Random.Decimal(1000, 50000))
                .RuleFor(r => r.BaggageIncome, f => f.Random.Decimal(100, 10000))
                .RuleFor(r => r.TotalIncome, f => f.Random.Decimal(1100, 60000));

            var rows = rowFaker.Generate(12);

            return new IncomeReportResultDTO
            {
                Rows = rows,
                Summary = new IncomeReportSummaryDTO
                {
                    TotalFlights = rows.Sum(r => r.Flights),
                    TotalFirstClass = rows.Sum(r => r.FirstClass),
                    TotalTouristClass = rows.Sum(r => r.TouristClass),
                    TotalPassengers = rows.Sum(r => r.TotalPassengers),
                    TotalTicketIncome = rows.Sum(r => r.TicketIncome),
                    TotalBaggageIncome = rows.Sum(r => r.BaggageIncome),
                    TotalIncome = rows.Sum(r => r.TotalIncome)
                }
            };
        }
    }
}
