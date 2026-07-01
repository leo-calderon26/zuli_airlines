using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Validation;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class FlightRouteServiceTests
    {
        private Mock<IFlightRouteRepository> _flightRouteRepositoryMock;
        private Mock<IValidator<FlightRouteDTO>> _flightRouteValidatorMock;
        private Mock<IUserRepository> _userRepositoryMock;

        private FlightRouteService _flightRouteService;

        [SetUp]
        public void SetUp()
        {
            _flightRouteRepositoryMock = new Mock<IFlightRouteRepository>();
            _flightRouteValidatorMock = new Mock<IValidator<FlightRouteDTO>>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _flightRouteService = new FlightRouteService(
                _flightRouteRepositoryMock.Object,
                _flightRouteValidatorMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Test]
        public async Task DeleteFlightRoute_WhenRouteHasNoReservations_ReturnsHardDeleteSuccess()
        {
            const int flightRouteId = 1;

            _flightRouteRepositoryMock
                .Setup(repository => repository.DeleteFlightRoute(flightRouteId))
                .ReturnsAsync(1);

            var result = await _flightRouteService.DeleteFlightRoute(flightRouteId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(
                result.Message,
                Is.EqualTo("La ruta se eliminó correctamente.")
            );

            _flightRouteRepositoryMock.Verify(
                repository => repository.DeleteFlightRoute(flightRouteId),
                Times.Once
            );
        }

        [Test]
        public async Task DeleteFlightRoute_WhenRouteHasReservations_ReturnsSoftDeleteSuccess()
        {
            const int flightRouteId = 2;

            _flightRouteRepositoryMock
                .Setup(repository => repository.DeleteFlightRoute(flightRouteId))
                .ReturnsAsync(2);

            var result = await _flightRouteService.DeleteFlightRoute(flightRouteId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(
                result.Message,
                Is.EqualTo(
                    "La ruta tenía compras o reservas asociadas, " +
                    "por lo que se deshabilitó conservando su historial."
                )
            );

            _flightRouteRepositoryMock.Verify(
                repository => repository.DeleteFlightRoute(flightRouteId),
                Times.Once
            );
        }

        [Test]
        public void DeleteFlightRoute_WhenRouteDoesNotExist_ThrowsZuliNotFoundException()
        {
            const int flightRouteId = 999;

            _flightRouteRepositoryMock
                .Setup(repository => repository.DeleteFlightRoute(flightRouteId))
                .ReturnsAsync(4);

            var exception = Assert.ThrowsAsync<ZuliNotFoundException>(
                async () => await _flightRouteService.DeleteFlightRoute(flightRouteId)
            );

            Assert.That(exception, Is.Not.Null);
            Assert.That(
                exception.Message,
                Does.Contain($"La ruta con ID {flightRouteId}")
            );

            _flightRouteRepositoryMock.Verify(
                repository => repository.DeleteFlightRoute(flightRouteId),
                Times.Once
            );
        }
    }
}