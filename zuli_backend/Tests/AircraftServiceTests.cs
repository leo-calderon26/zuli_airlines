using Moq;
using NUnit.Framework;
using Mapster;
using System;
using System.Threading.Tasks;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Mappings;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class AircraftServiceTests
    {
        private Mock<IAircraftRepository> _aircraftRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;

        private AircraftService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            new AircraftMappingConfig().Register(config);

            _aircraftRepositoryMock = new Mock<IAircraftRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _service = new AircraftService(_aircraftRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task UpdateAircraft_AdminAndExistingAircraft_UpdatesSuccessfully()
        {
            var aircraftId = Guid.NewGuid();
            var request = BuildValidAircraftDto();
            var existingAircraft = new AircraftEntity
            {
                aircraftId = aircraftId,
                model = "OldModel",
                weight = 900m,
                baggageCapacity = 270m,
                numberEconomyClassRows = 5,
                numberSeatingRowsEconomy = 2,
                numberFirstClassRows = 2,
                numberSeatingRowsFirst = 2,
                AdminId = Guid.NewGuid()
            };

            _userRepositoryMock.Setup(r => r.IsAdmin(request.businessId!)).ReturnsAsync(true);
            _aircraftRepositoryMock.Setup(r => r.GetById(aircraftId)).ReturnsAsync(existingAircraft);
            _aircraftRepositoryMock.Setup(r => r.UpdateAircraftAsync(It.IsAny<AircraftEntity>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateAircraftAsync(aircraftId, request);

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Se actualizó la aeronave correctamente"));
            Assert.That(existingAircraft.model, Is.EqualTo("B737"));
            Assert.That(existingAircraft.weight, Is.EqualTo(1000m));
            Assert.That(existingAircraft.baggageCapacity, Is.EqualTo(300m));

            _aircraftRepositoryMock.Verify(r => r.UpdateAircraftAsync(existingAircraft), Times.Once);
        }

        [Test]
        public void UpdateAircraft_NonAdminUser_ThrowsUnauthorizedAndDoesNotUpdate()
        {
            var aircraftId = Guid.NewGuid();
            var request = BuildValidAircraftDto();

            _userRepositoryMock.Setup(r => r.IsAdmin(request.businessId!)).ReturnsAsync(false);

            Assert.That(async () => await _service.UpdateAircraftAsync(aircraftId, request), Throws.TypeOf<ZuliUnauthorizedException>());

            _aircraftRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            _aircraftRepositoryMock.Verify(r => r.UpdateAircraftAsync(It.IsAny<AircraftEntity>()), Times.Never);
        }

        [Test]
        public void UpdateAircraft_AircraftDoesNotExist_ThrowsNotFound()
        {
            var aircraftId = Guid.NewGuid();
            var request = BuildValidAircraftDto();

            _userRepositoryMock.Setup(r => r.IsAdmin(request.businessId!)).ReturnsAsync(true);
            _aircraftRepositoryMock.Setup(r => r.GetById(aircraftId)).ReturnsAsync((AircraftEntity?)null);

            Assert.That(async () => await _service.UpdateAircraftAsync(aircraftId, request), Throws.TypeOf<ZuliNotFoundException>());

            _aircraftRepositoryMock.Verify(r => r.UpdateAircraftAsync(It.IsAny<AircraftEntity>()), Times.Never);
        }


        [Test]
        public void UpdateAircraft_InvalidRequest_ThrowsValidationException()
        {
            var aircraftId = Guid.NewGuid();
            var request = new AircraftDTO
            {
                businessId = "123456789",
                model = "Invalid!!",
                weight = -1,
                baggageCapacity = -1,
                numberEconomyClassRows = -1,
                numberSeatingRowsEconomy = -1,
                numberFirstClassRows = -1,
                numberSeatingRowsFirst = -1
            };

            Assert.That(async () => await _service.UpdateAircraftAsync(aircraftId, request), Throws.TypeOf<ZuliValidationException>());

            _userRepositoryMock.Verify(r => r.IsAdmin(It.IsAny<string>()), Times.Never);
            _aircraftRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            _aircraftRepositoryMock.Verify(r => r.UpdateAircraftAsync(It.IsAny<AircraftEntity>()), Times.Never);
        }

        private static AircraftDTO BuildValidAircraftDto()
        {
            return new AircraftDTO
            {
                businessId = "123456789",
                model = "B737",
                weight = 1000m,
                baggageCapacity = 300m,
                numberEconomyClassRows = 10,
                numberSeatingRowsEconomy = 2,
                numberFirstClassRows = 5,
                numberSeatingRowsFirst = 2
            };
        }
    }
}