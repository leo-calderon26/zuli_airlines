using MapsterMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class PurchaseEmailService : IPurchaseEmailService
    {
        private readonly IPurchaseConfirmationRepository _purchaseConfirmationRepository;
        private readonly IPurchaseConfirmationPdfService _purchaseConfirmationPdfService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public PurchaseEmailService(
            IPurchaseConfirmationRepository purchaseConfirmationRepository,
            IPurchaseConfirmationPdfService purchaseConfirmationPdfService,
            IEmailService emailService,
            IMapper mapper)
        {
            _purchaseConfirmationRepository = purchaseConfirmationRepository;
            _purchaseConfirmationPdfService = purchaseConfirmationPdfService;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task SendPurchaseEmailsAsync(string reservationCode, CancellationToken cancellationToken = default)
        {
            var confirmation = await _purchaseConfirmationRepository
                .GetPurchaseConfirmationAsync(reservationCode);

            if (confirmation == null)
            {
                throw new ZuliNotFoundException("No se encontró la confirmación de compra.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var confirmationDto = _mapper.Map<PurchaseConfirmationPageDTO>(confirmation);
            confirmationDto.Breakdown = PurchaseConfirmationService.BuildPurchaseBreakdown(confirmationDto);

            await SendPurchaseEmailsAsync(confirmationDto, cancellationToken);
        }

        public async Task SendPurchaseEmailsAsync(PurchaseConfirmationPageDTO confirmation, CancellationToken cancellationToken = default)
        {
            var invoicePdf = _purchaseConfirmationPdfService.GenerateInvoicePdf(confirmation);
            var confirmationPdf = _purchaseConfirmationPdfService.GenerateConfirmationPdf(confirmation);

            cancellationToken.ThrowIfCancellationRequested();

            await _emailService.SendInvoiceEmailAsync(
                confirmation.BuyerEmail,
                confirmation.BuyerName,
                confirmation.ReservationCode,
                invoicePdf
            );

            await _emailService.SendPurchaseConfirmationEmailAsync(
                confirmation.BuyerEmail,
                confirmation.BuyerName,
                confirmation.ReservationCode,
                confirmationPdf
            );
        }
    }
}
