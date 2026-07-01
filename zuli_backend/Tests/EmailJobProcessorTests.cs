using Moq;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Test
{
    [TestFixture]
    public class EmailJobProcessorTests
    {
        [Test]
        public async Task ProcessAsync_PurchaseConfirmationJob_SendsPurchaseEmails()
        {
            var purchaseEmailServiceMock = new Mock<IPurchaseEmailService>();
            var additionalBaggageEmailServiceMock = new Mock<IAdditionalBaggageEmailService>();
            var processor = new EmailJobProcessor(
                purchaseEmailServiceMock.Object,
                additionalBaggageEmailServiceMock.Object
            );
            var job = new EmailJobDTO
            {
                Type = EmailJobType.PurchaseConfirmation,
                ReservationCode = "ZUTEST01"
            };

            await processor.ProcessAsync(job);

            purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync("ZUTEST01", It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Test]
        public async Task ProcessAsync_AdditionalBaggagePurchaseJob_SendsAdditionalBaggageEmail()
        {
            var purchaseEmailServiceMock = new Mock<IPurchaseEmailService>();
            var additionalBaggageEmailServiceMock = new Mock<IAdditionalBaggageEmailService>();
            var processor = new EmailJobProcessor(
                purchaseEmailServiceMock.Object,
                additionalBaggageEmailServiceMock.Object
            );
            var job = new EmailJobDTO
            {
                Type = EmailJobType.AdditionalBaggagePurchase,
                ReservationCode = "ZUTEST01",
                AdditionalCheckedBaggage = 2,
                AdditionalCarryOn = 1,
                AdditionalBaggageTotal = 150m,
                ReservationTotal = 1200m
            };

            await processor.ProcessAsync(job);

            additionalBaggageEmailServiceMock.Verify(
                service => service.SendAdditionalBaggagePurchaseEmailAsync(job, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
