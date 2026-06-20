using Bogus;
using MapsterMapper;
using Moq;
using zuli_Business;
using zuli_Business.DTO.Filters;
using zuli_Data.Entities.Filters;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class FilterOptionsServiceTests
    {
        private Mock<IFilterOptionsRepository> _filterOptionsRepositoryMock;
        private Mock<IMapper> _mapperMock;

        private FilterOptionsService _filterOptionsService;

        [SetUp]
        public void SetUp()
        {
            Randomizer.Seed = new Random(12345);

            _filterOptionsRepositoryMock = new Mock<IFilterOptionsRepository>();
            _mapperMock = new Mock<IMapper>();

            _filterOptionsService = new FilterOptionsService(
                _filterOptionsRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetFilterOptionsAsync_ReturnsMappedDto()
        {
            var entity = BuildValidFilterOptionsEntity();
            var expectedDto = BuildValidFilterOptionsDto();

            _filterOptionsRepositoryMock
                .Setup(repository => repository.GetFilterOptionsAsync())
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(mapper => mapper.Map<FilterOptionsDTO>(entity))
                .Returns(expectedDto);

            var result = await _filterOptionsService.GetFilterOptionsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expectedDto));

            _filterOptionsRepositoryMock.Verify(
                repository => repository.GetFilterOptionsAsync(),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<FilterOptionsDTO>(entity),
                Times.Once
            );
        }

        [Test]
        public async Task GetFilterOptionsAsync_WithEmptyLists_ReturnsMappedDto()
        {
            var entity = new FilterOptionsEntity
            {
                Years = new List<int>(),
                Origins = new List<AirportOptionEntity>(),
                Destinations = new List<AirportOptionEntity>(),
                Airlines = new List<AirlineOptionEntity>(),
                Classes = new List<FlightClassOptionEntity>()
            };

            var expectedDto = new FilterOptionsDTO
            {
                Years = new List<int>(),
                Origins = new List<AirportOptionDTO>(),
                Destinations = new List<AirportOptionDTO>(),
                Airlines = new List<AirlineOptionDTO>(),
                Classes = new List<FlightClassOptionDTO>()
            };

            _filterOptionsRepositoryMock
                .Setup(repository => repository.GetFilterOptionsAsync())
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(mapper => mapper.Map<FilterOptionsDTO>(entity))
                .Returns(expectedDto);

            var result = await _filterOptionsService.GetFilterOptionsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Years, Is.Empty);
            Assert.That(result.Origins, Is.Empty);
            Assert.That(result.Destinations, Is.Empty);
            Assert.That(result.Airlines, Is.Empty);
            Assert.That(result.Classes, Is.Empty);
        }

        [Test]
        public async Task GetFilterOptionsAsync_CallsMapperWithRepositoryResult()
        {
            var entity = BuildValidFilterOptionsEntity();

            _filterOptionsRepositoryMock
                .Setup(repository => repository.GetFilterOptionsAsync())
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(mapper => mapper.Map<FilterOptionsDTO>(entity))
                .Returns(new FilterOptionsDTO());

            await _filterOptionsService.GetFilterOptionsAsync();

            _mapperMock.Verify(
                mapper => mapper.Map<FilterOptionsDTO>(entity),
                Times.Once
            );
        }

        private static FilterOptionsEntity BuildValidFilterOptionsEntity()
        {
            var airportFaker = new Faker<AirportOptionEntity>()
                .RuleFor(a => a.AirportCode, f => f.PickRandom("SJO", "MIA", "PTY", "LAX", "MAD", "JFK"));

            var airlineFaker = new Faker<AirlineOptionEntity>()
                .RuleFor(a => a.AirlineCode, f => f.Random.String2(1, "1"))
                .RuleFor(a => a.AirlineName, f => f.Company.CompanyName());

            var classFaker = new Faker<FlightClassOptionEntity>()
                .RuleFor(c => c.Value, f => f.PickRandom("Economy", "FirstClass", "Business"))
                .RuleFor(c => c.Label, f => f.PickRandom("Turista", "Primera Clase", "Ejecutiva"));

            return new FilterOptionsEntity
            {
                Years = Enumerable.Range(0, 5).Select(_ => new Faker().Random.Int(2020, 2030)).Distinct().ToList(),
                Origins = airportFaker.Generate(3),
                Destinations = airportFaker.Generate(3),
                Airlines = airlineFaker.Generate(2),
                Classes = classFaker.Generate(3)
            };
        }

        private static FilterOptionsDTO BuildValidFilterOptionsDto()
        {
            var airportFaker = new Faker<AirportOptionDTO>()
                .RuleFor(a => a.AirportCode, f => f.PickRandom("SJO", "MIA", "PTY", "LAX", "MAD", "JFK"));

            var airlineFaker = new Faker<AirlineOptionDTO>()
                .RuleFor(a => a.AirlineCode, f => f.Random.String2(1, "1"))
                .RuleFor(a => a.AirlineName, f => f.Company.CompanyName());

            var classFaker = new Faker<FlightClassOptionDTO>()
                .RuleFor(c => c.Value, f => f.PickRandom("Economy", "FirstClass", "Business"))
                .RuleFor(c => c.Label, f => f.PickRandom("Turista", "Primera Clase", "Ejecutiva"));

            return new FilterOptionsDTO
            {
                Years = Enumerable.Range(0, 5).Select(_ => new Faker().Random.Int(2020, 2030)).Distinct().ToList(),
                Origins = airportFaker.Generate(3),
                Destinations = airportFaker.Generate(3),
                Airlines = airlineFaker.Generate(2),
                Classes = classFaker.Generate(3)
            };
        }
    }
}
