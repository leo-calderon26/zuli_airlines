using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using zuli_Business;
using zuli_Business.DTO.Cancellation;
using zuli_Business.Interface;
using zuli_Data.Enums;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class ReservationCancellationServiceTests
    {
        private const string VALID_RESERVATION_CODE = "ABC12345";
        private const string CANCELLED_RESERVATION_CODE = "CAN12345";
        private const string NON_EXISTENT_CODE = "XXX99999";
        private const string VALID_TOKEN = "some-valid-token-string-used-in-tests";

        private Mock<IReservationCancellationRepository> _repositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IConfiguration> _configurationMock;

        private ReservationCancellationService _service;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IReservationCancellationRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock
                .Setup(configuration => configuration["Frontend:BaseUrl"])
                .Returns("https://zuli.example.com");

            _service = new ReservationCancellationService(
                _repositoryMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object
            );
        }

        [Test]
        public async Task RequestCancellationAsync_ActiveReservation_SendsEmailAndReturnsSuccess()
        {
            var request = new RequestCancellationRequestDTO
            {
                ReservationCode = VALID_RESERVATION_CODE
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetCancellationInfoAsync(VALID_RESERVATION_CODE))
                .ReturnsAsync((
                    "buyer@example.com",
                    "Juan Pérez",
                    (int)ReservationStatus.Active,
                    42
                ));

            _repositoryMock
                .Setup(repository =>
                    repository.SetCancellationTokenAsync(
                        VALID_RESERVATION_CODE,
                        It.IsAny<string>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()
                    ))
                .ReturnsAsync(1);

            _emailServiceMock
                .Setup(emailService =>
                    emailService.SendCancellationRequestEmailAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ))
                .Returns(Task.CompletedTask);

            var result = await _service.RequestCancellationAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(
                result.Message,
                Does.Contain("instrucciones al correo del comprador")
            );

            _emailServiceMock.Verify(
                emailService =>
                    emailService.SendCancellationRequestEmailAsync(
                        "buyer@example.com",
                        "Juan Pérez",
                        It.IsAny<string>()
                    ),
                Times.Once
            );

            _repositoryMock.Verify(
                repository =>
                    repository.ConfirmCancellationAsync(It.IsAny<string>()),
                Times.Never
            );
        }

        [Test]
        public void RequestCancellationAsync_ReservationNotFound_ThrowsZuliNotFoundException()
        {
            var request = new RequestCancellationRequestDTO
            {
                ReservationCode = NON_EXISTENT_CODE
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetCancellationInfoAsync(NON_EXISTENT_CODE))
                .ReturnsAsync(
                    ((string? BuyerEmail, string? BuyerName, int ReservationStatusId, int ReservationId)?)null
                );

            Assert.That(
                async () => await _service.RequestCancellationAsync(request),
                Throws.TypeOf<ZuliNotFoundException>()
            );

            _emailServiceMock.Verify(
                emailService =>
                    emailService.SendCancellationRequestEmailAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task RequestCancellationAsync_CancelledReservation_ReturnsMessageWithoutSendingEmail()
        {
            var request = new RequestCancellationRequestDTO
            {
                ReservationCode = CANCELLED_RESERVATION_CODE
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetCancellationInfoAsync(CANCELLED_RESERVATION_CODE))
                .ReturnsAsync((
                    "buyer@example.com",
                    "Juan Pérez",
                    (int)ReservationStatus.Cancelled,
                    10
                ));

            var result = await _service.RequestCancellationAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(
                result.Message,
                Does.Contain("ya se encuentra cancelada")
            );

            _emailServiceMock.Verify(
                emailService =>
                    emailService.SendCancellationRequestEmailAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    ),
                Times.Never
            );
        }

        [Test]
        public async Task ConfirmCancellationAsync_ValidToken_ReturnsSuccess()
        {
            var request = new ConfirmCancellationRequestDTO
            {
                Token = VALID_TOKEN
            };

            _repositoryMock
                .Setup(repository =>
                    repository.ConfirmCancellationAsync(It.IsAny<string>()))
                .ReturnsAsync(CancellationConfirmationResult.Confirmed);

            var result = await _service.ConfirmCancellationAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(
                result.Message,
                Does.Contain("cancelada correctamente")
            );
        }

        [Test]
        public void ConfirmCancellationAsync_InvalidOrExpiredToken_ThrowsZuliValidationException()
        {
            var request = new ConfirmCancellationRequestDTO
            {
                Token = VALID_TOKEN
            };

            _repositoryMock
                .Setup(repository =>
                    repository.ConfirmCancellationAsync(It.IsAny<string>()))
                .ReturnsAsync(
                    CancellationConfirmationResult.InvalidOrExpiredOrUsed
                );

            Assert.That(
                async () => await _service.ConfirmCancellationAsync(request),
                Throws.TypeOf<ZuliValidationException>()
            );
        }
    }
}