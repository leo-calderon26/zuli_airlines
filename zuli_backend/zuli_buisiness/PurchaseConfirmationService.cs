using FluentValidation;
using MapsterMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class PurchaseConfirmationService : IPurchaseConfirmationService
    {
        private readonly IPurchaseConfirmationRepository _purchaseConfirmationRepository;
        private readonly IPurchaseConfirmationPdfService _purchaseConfirmationPdfService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IValidator<PurchaseConfirmationPageDTO> _purchaseConfirmationValidator;

        public PurchaseConfirmationService(
            IPurchaseConfirmationRepository purchaseConfirmationRepository,
            IPurchaseConfirmationPdfService purchaseConfirmationPdfService,
            IEmailService emailService,
            IMapper mapper,
            IValidator<PurchaseConfirmationPageDTO> purchaseConfirmationValidator)
        {
            _purchaseConfirmationRepository = purchaseConfirmationRepository;
            _purchaseConfirmationPdfService = purchaseConfirmationPdfService;
            _emailService = emailService;
            _mapper = mapper;
            _purchaseConfirmationValidator = purchaseConfirmationValidator;
        }

        public async Task<PurchaseConfirmationPageDTO> GetConfirmationPageAsync(string reservationCode)
        {
            var confirmationEntity = await _purchaseConfirmationRepository.GetPurchaseConfirmationAsync(reservationCode);

            if (confirmationEntity == null)
            {
                throw new ZuliNotFoundException("No se encontró la reserva indicada.");
            }

            return _mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity);
        }

        public async Task<PurchaseConfirmationPageDTO> CompleteConfirmationAsync(string reservationCode)
        {
            var confirmationEntity = await _purchaseConfirmationRepository.GetPurchaseConfirmationAsync(reservationCode);

            if (confirmationEntity == null)
            {
                throw new ZuliNotFoundException("No se encontró la reserva indicada.");
            }

            var confirmation = _mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity);

            ValidateConfirmationData(confirmation);

            byte[] invoicePdf = _purchaseConfirmationPdfService.GenerateInvoicePdf(confirmation);
            byte[] confirmationPdf = _purchaseConfirmationPdfService.GenerateConfirmationPdf(confirmation);

            try
            {
                await _emailService.SendInvoiceEmailAsync(
                    confirmation.BuyerEmail,
                    confirmation.BuyerName,
                    confirmation.ReservationCode,
                    invoicePdf
                );

                confirmation.InvoiceEmailSent = true;

                await _emailService.SendPurchaseConfirmationEmailAsync(
                    confirmation.BuyerEmail,
                    confirmation.BuyerName,
                    confirmation.ReservationCode,
                    confirmationPdf
                );

                confirmation.ConfirmationEmailSent = true;
            }
            catch
            {
                throw new ZuliEmailException(
                    "Error al enviar el correo de confirmación. Por favor contacte a la aerolínea."
                );
            }

            confirmation.Message = "Reserva completada. Los detalles han sido enviados a su correo electrónico.";

            return confirmation;
        }

        private void ValidateConfirmationData(PurchaseConfirmationPageDTO confirmation)
        {
            var validationResult = _purchaseConfirmationValidator.Validate(confirmation);

            if (validationResult.IsValid)
            {
                return;
            }

            var firstError = validationResult.Errors.First();

            throw new ZuliValidationException(
                firstError.PropertyName,
                firstError.ErrorMessage
            );
        }
    }
}