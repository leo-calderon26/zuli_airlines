using Moq;
using NUnit.Framework;
using Mapster;
using System;
using System.Threading.Tasks;
using FluentValidation;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Mappings;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class AirportServiceTests
    {
        private Mock<IAirportRepository> _airportRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;

        private AirportService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            new AirportMappingConfig().Register(config);

            _airportRepositoryMock = new Mock<IAirportRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _service = new AirportService(
                _airportRepositoryMock.Object,
                _userRepositoryMock.Object,
                new AirportValidator());
        }

        [Test]
        public async Task UpdateAirport_AdminAndExistingAirport_UpdatesSuccessfully()
        {
            var airportCode = "sjo";
            var airport = BuildValidAirportDto();
            var existingAirport = new AirportEntity
            {
                AirportCode = "SJO",
                Name = "Old",
                Country = "Costa Rica",
                City = "San Jose",
                AdminId = Guid.NewGuid()
            };

            _userRepositoryMock.Setup(r => r.IsAdmin(airport.businessId)).ReturnsAsync(true);
            _airportRepositoryMock.Setup(r => r.GetByCodeAsync("SJO")).ReturnsAsync(existingAirport);


            _airportRepositoryMock.Setup(r => r.UpdateAirportAsync(It.IsAny<AirportEntity>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateAirportAsync(airportCode, airport);

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Aeropuerto actualizado correctamente"));
            Assert.That(existingAirport.Name, Is.EqualTo("Juan Santamaria"));
            Assert.That(existingAirport.Country, Is.EqualTo("Costa Rica"));
            Assert.That(existingAirport.City, Is.EqualTo("Alajuela"));

            _airportRepositoryMock.Verify(r => r.UpdateAirportAsync(existingAirport), Times.Once);
        }

        [Test]
        public void UpdateAirport_NonAdminUser_ThrowsUnauthorizedAndDoesNotUpdate()
        {
            var airport = BuildValidAirportDto();

            _userRepositoryMock.Setup(r => r.IsAdmin(airport.businessId)).ReturnsAsync(false);

            Assert.That(async () => await _service.UpdateAirportAsync("SJO", airport), Throws.TypeOf<ZuliUnauthorizedException>());

            _airportRepositoryMock.Verify(r => r.GetByCodeAsync(It.IsAny<string>()), Times.Never);
            _airportRepositoryMock.Verify(r => r.UpdateAirportAsync(It.IsAny<AirportEntity>()), Times.Never);
        }

        [Test]
        public void UpdateAirport_AirportDoesNotExist_ThrowsNotFound()
        {
            var airport = BuildValidAirportDto();

            _userRepositoryMock.Setup(r => r.IsAdmin(airport.businessId)).ReturnsAsync(true);
            _airportRepositoryMock.Setup(r => r.GetByCodeAsync("SJO")).ReturnsAsync((AirportEntity?)null);

            Assert.That(async () => await _service.UpdateAirportAsync(" sjo ", airport), Throws.TypeOf<ZuliNotFoundException>());

            _airportRepositoryMock.Verify(r => r.UpdateAirportAsync(It.IsAny<AirportEntity>()), Times.Never);
        }

        [Test]
        public void UpdateAirport_InvalidRequest_ThrowsValidationException()
        {
            var airport = new AirportDTO
            {
                airportCode = "SJ",
                name = "",
                country = "Costa Rica",
                city = "Alajuela",
                businessId = "123456789"
            };

            var existingAirport = new AirportEntity
            {
                AirportCode = "SJO",
                Name = "Old",
                Country = "Costa Rica",
                City = "San Jose",
                AdminId = Guid.NewGuid()
            };

            _userRepositoryMock.Setup(r => r.IsAdmin(airport.businessId)).ReturnsAsync(true);
            _airportRepositoryMock.Setup(r => r.GetByCodeAsync("SJO")).ReturnsAsync(existingAirport);

            Assert.That(async () => await _service.UpdateAirportAsync("SJO", airport), Throws.TypeOf<ZuliValidationException>());

            _airportRepositoryMock.Verify(r => r.GetByCodeAsync("SJO"), Times.Once);
            _airportRepositoryMock.Verify(r => r.UpdateAirportAsync(It.IsAny<AirportEntity>()), Times.Never);
        }

        private static AirportDTO BuildValidAirportDto(string? airportCode = null)
        {
            return new AirportDTO
            {
                airportCode = airportCode ?? "SJO",
                name = "Juan Santamaria",
                country = "Costa Rica",
                city = "Alajuela",
                businessId = "123456789"
            };
        }

        [Test]
        public void DeleteAirport_AirportDoesNotExist_ThrowsNotFound()
        {
            var airportCode = "XYZ";

            _airportRepositoryMock
                .Setup(r => r.GetByCodeAsync(airportCode))
                .ReturnsAsync((AirportEntity?)null);

            Assert.That(async () => await _service.DeleteAirport(airportCode),
                Throws.TypeOf<ZuliNotFoundException>());

            _airportRepositoryMock.Verify(r => r.GetByCodeAsync(airportCode), Times.Once);
            _airportRepositoryMock.Verify(r => r.DeleteAirport(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteAirport_ExistingAirport_DeletesSuccessfully()
        {
            var airportCode = "SJO";
            var existingAirport = new AirportEntity
            {
                AirportCode = airportCode,
                Name = "Juan Santamaría International",
                AdminId = Guid.NewGuid()
            };

            _airportRepositoryMock
                .Setup(r => r.GetByCodeAsync(airportCode))
                .ReturnsAsync(existingAirport);

            _airportRepositoryMock
                .Setup(r => r.DeleteAirport(airportCode))
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAirport(airportCode);

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Se eliminó el aeropuerto correctamente"));

            _airportRepositoryMock.Verify(r => r.GetByCodeAsync(airportCode), Times.Once);
            _airportRepositoryMock.Verify(r => r.DeleteAirport(airportCode), Times.Once);
        }
    }
}