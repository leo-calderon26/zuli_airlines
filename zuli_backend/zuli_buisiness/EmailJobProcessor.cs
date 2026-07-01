using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class EmailJobProcessor : IEmailJobProcessor
    {
        private readonly IPurchaseEmailService _purchaseEmailService;
        private readonly IAdditionalBaggageEmailService _additionalBaggageEmailService;

        public EmailJobProcessor(
            IPurchaseEmailService purchaseEmailService,
            IAdditionalBaggageEmailService additionalBaggageEmailService)
        {
            _purchaseEmailService = purchaseEmailService;
            _additionalBaggageEmailService = additionalBaggageEmailService;
        }

        public async Task ProcessAsync(EmailJobDTO job, CancellationToken cancellationToken = default)
        {
            switch (job.Type)
            {
                case EmailJobType.PurchaseConfirmation:
                    await _purchaseEmailService.SendPurchaseEmailsAsync(job.ReservationCode, cancellationToken);
                    break;
                case EmailJobType.AdditionalBaggagePurchase:
                    await _additionalBaggageEmailService.SendAdditionalBaggagePurchaseEmailAsync(job, cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported email job type: {job.Type}");
            }
        }
    }
}
