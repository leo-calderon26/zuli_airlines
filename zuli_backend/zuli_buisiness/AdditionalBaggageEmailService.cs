using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class AdditionalBaggageEmailService : IAdditionalBaggageEmailService
    {
        private readonly IPurchaseConfirmationRepository _purchaseConfirmationRepository;
        private readonly IEmailService _emailService;

        public AdditionalBaggageEmailService(
            IPurchaseConfirmationRepository purchaseConfirmationRepository,
            IEmailService emailService)
        {
            _purchaseConfirmationRepository = purchaseConfirmationRepository;
            _emailService = emailService;
        }

        public async Task SendAdditionalBaggagePurchaseEmailAsync(EmailJobDTO job, CancellationToken cancellationToken = default)
        {
            var confirmation = await _purchaseConfirmationRepository
                .GetPurchaseConfirmationAsync(job.ReservationCode);

            if (confirmation == null)
            {
                throw new ZuliNotFoundException("No se encontró la confirmación de compra.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _emailService.SendAdditionalBaggagePurchaseEmailAsync(
                confirmation.BuyerEmail,
                confirmation.BuyerName,
                job.ReservationCode,
                job.AdditionalCheckedBaggage,
                job.AdditionalCarryOn,
                job.AdditionalBaggageTotal,
                job.ReservationTotal
            );
        }
    }
}
