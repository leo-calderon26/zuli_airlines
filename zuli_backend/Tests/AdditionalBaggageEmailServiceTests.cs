using Moq;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Test
{
    [TestFixture]
    public class AdditionalBaggageEmailServiceTests
    {
        private Mock<IPurchaseConfirmationRepository> _purchaseConfirmationRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private AdditionalBaggageEmailService _additionalBaggageEmailService;

        [SetUp]
        public void SetUp()
        {
            _purchaseConfirmationRepositoryMock = new Mock<IPurchaseConfirmationRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            _additionalBaggageEmailService = new AdditionalBaggageEmailService(
                _purchaseConfirmationRepositoryMock.Object,
                _emailServiceMock.Object
            );
        }

        [Test]
        public async Task SendAdditionalBaggagePurchaseEmailAsync_ValidReservation_SendsEmailToBuyer()
        {
            var job = new EmailJobDTO
            {
                Type = EmailJobType.AdditionalBaggagePurchase,
                ReservationCode = "ZUTEST01",
                AdditionalCheckedBaggage = 2,
                AdditionalCarryOn = 1,
                AdditionalBaggageTotal = 175m,
                ReservationTotal = 1200m
            };

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(job.ReservationCode))
                .ReturnsAsync(new PurchaseConfirmationEntity
                {
                    ReservationCode = job.ReservationCode,
                    BuyerEmail = "buyer@test.com",
                    BuyerName = "Valeria Jimenez Castro"
                });

            await _additionalBaggageEmailService.SendAdditionalBaggagePurchaseEmailAsync(job);

            _emailServiceMock.Verify(
                emailService => emailService.SendAdditionalBaggagePurchaseEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    job.ReservationCode,
                    2,
                    1,
                    175m,
                    1200m
                ),
                Times.Once
            );
        }

        [Test]
        public void SendAdditionalBaggagePurchaseEmailAsync_ReservationDoesNotExist_ThrowsZuliNotFoundException()
        {
            var job = new EmailJobDTO
            {
                Type = EmailJobType.AdditionalBaggagePurchase,
                ReservationCode = "INVALID1"
            };

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(job.ReservationCode))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            Assert.That(
                async () => await _additionalBaggageEmailService.SendAdditionalBaggagePurchaseEmailAsync(job),
                Throws.TypeOf<ZuliNotFoundException>()
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendAdditionalBaggagePurchaseEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>()
                ),
                Times.Never
            );
        }
    }
}
