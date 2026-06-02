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

        public PurchaseConfirmationService(
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

        public async Task<PurchaseConfirmationPageDTO?> GetConfirmationPageAsync(int reservationId)
        {
            var confirmationEntity = await _purchaseConfirmationRepository.GetPurchaseConfirmationAsync(reservationId);

            if (confirmationEntity == null)
            {
                return null;
            }

            return _mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity);
        }

        public async Task<PurchaseConfirmationPageDTO> CompleteConfirmationAsync(int reservationId)
        {
            var confirmationEntity = await _purchaseConfirmationRepository.GetPurchaseConfirmationAsync(reservationId);

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

        private static void ValidateConfirmationData(PurchaseConfirmationPageDTO confirmation)
        {
            if (string.IsNullOrWhiteSpace(confirmation.ReservationCode))
            {
                throw new ZuliValidationException(
                    "reservationCode",
                    "La reserva no tiene código de reserva."
                );
            }

            if (string.IsNullOrWhiteSpace(confirmation.BuyerEmail))
            {
                throw new ZuliValidationException(
                    "buyerEmail",
                    "La reserva no tiene correo del comprador."
                );
            }

            if (string.IsNullOrWhiteSpace(confirmation.BuyerName))
            {
                throw new ZuliValidationException(
                    "buyerName",
                    "La reserva no tiene nombre del comprador."
                );
            }

            if (confirmation.Passengers.Count == 0)
            {
                throw new ZuliValidationException(
                    "passengers",
                    "La reserva no tiene pasajeros asociados."
                );
            }

            if (confirmation.Flights.Count == 0)
            {
                throw new ZuliValidationException(
                    "flights",
                    "La reserva no tiene vuelos asociados."
                );
            }
        }
    }
}