using System.Net;
using System.Text;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class PurchaseConfirmationService : IPurchaseConfirmationService
    {
        private readonly IPurchaseConfirmationRepository _purchaseConfirmationRepository;
        private readonly IEmailService _emailService;

        public PurchaseConfirmationService(
            IPurchaseConfirmationRepository purchaseConfirmationRepository,
            IEmailService emailService)
        {
            _purchaseConfirmationRepository = purchaseConfirmationRepository;
            _emailService = emailService;
        }

        public async Task<PurchaseConfirmationPageDTO?> GetConfirmationPageAsync(Guid reservationId)
        {
            return await _purchaseConfirmationRepository.GetPurchaseConfirmationAsync(reservationId);
        }

        public async Task<PurchaseConfirmationPageDTO> CompleteConfirmationAsync(Guid reservationId)
        {
            var confirmation = await _purchaseConfirmationRepository.GetPurchaseConfirmationAsync(reservationId);

            if (confirmation is null)
            {
                throw new ZuliNotFoundException("No se encontró la reserva indicada.");
            }

            ValidateConfirmationData(confirmation);

            string invoiceBody = BuildInvoiceBody(confirmation);
            string confirmationBody = BuildConfirmationBody(confirmation);

            try
            {
                await _emailService.SendInvoiceEmailAsync(
                    confirmation.BuyerEmail,
                    confirmation.BuyerName,
                    confirmation.ReservationCode,
                    invoiceBody
                );

                confirmation.InvoiceEmailSent = true;

                await _emailService.SendPurchaseConfirmationEmailAsync(
                    confirmation.BuyerEmail,
                    confirmation.BuyerName,
                    confirmation.ReservationCode,
                    confirmationBody
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

        private static string BuildInvoiceBody(PurchaseConfirmationPageDTO confirmation)
        {
            string safeBuyerName = WebUtility.HtmlEncode(confirmation.BuyerName);
            string safeBuyerEmail = WebUtility.HtmlEncode(confirmation.BuyerEmail);
            string safeBuyerPhone = WebUtility.HtmlEncode(confirmation.BuyerPhone);
            string safePaymentMethod = WebUtility.HtmlEncode(confirmation.PaymentMethod);
            string safeFlightClass = WebUtility.HtmlEncode(confirmation.FlightClass);

            return $@"
                <h4>Datos del comprador</h4>

                <table style='width: 100%; border-collapse: collapse; margin-top: 12px;'>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'><strong>Comprador</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safeBuyerName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'><strong>Correo</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safeBuyerEmail}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'><strong>Teléfono</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safeBuyerPhone}</td>
                    </tr>
                </table>

                <h4 style='margin-top: 28px;'>Detalles del pago</h4>

                <table style='width: 100%; border-collapse: collapse; margin-top: 12px;'>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'><strong>Método de pago</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safePaymentMethod}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'><strong>Clase</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safeFlightClass}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'><strong>Total pagado</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{confirmation.TotalAmount:C}</td>
                    </tr>
                </table>";
        }

        private static string BuildConfirmationBody(PurchaseConfirmationPageDTO confirmation)
        {
            var body = new StringBuilder();

            body.AppendLine("<h3 style='margin-top: 28px;'>Información de pasajeros e itinerario</h3>");

            body.AppendLine("<h4>Pasajeros</h4>");
            body.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
            body.AppendLine(@"
                <tr>
                    <th style='text-align: left; padding: 8px; border-bottom: 1px solid #e5e7eb;'>Nombre completo</th>
                    <th style='text-align: left; padding: 8px; border-bottom: 1px solid #e5e7eb;'>Nacimiento</th>
                    <th style='text-align: left; padding: 8px; border-bottom: 1px solid #e5e7eb;'>Género</th>
                    <th style='text-align: left; padding: 8px; border-bottom: 1px solid #e5e7eb;'>País pasaporte</th>
                    <th style='text-align: left; padding: 8px; border-bottom: 1px solid #e5e7eb;'>Equipaje</th>
                </tr>");

            foreach (var passenger in confirmation.Passengers)
            {
                string safeFullName = WebUtility.HtmlEncode(passenger.FullName);
                string safeGender = WebUtility.HtmlEncode(passenger.Gender);
                string safePassportCountry = WebUtility.HtmlEncode(passenger.PassportCountry);

                body.AppendLine($@"
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safeFullName}</td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{passenger.BirthDate:dd/MM/yyyy}</td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safeGender}</td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>{safePassportCountry}</td>
                        <td style='padding: 8px; border-bottom: 1px solid #e5e7eb;'>
                            Maletas: {passenger.CheckedBaggageQuantity} / Carry on: {passenger.CarryOnQuantity}
                        </td>
                    </tr>");
            }

            body.AppendLine("</table>");

            body.AppendLine("<h4 style='margin-top: 28px;'>Itinerario</h4>");

            foreach (var flight in confirmation.Flights)
            {
                string safeFlightNumber = WebUtility.HtmlEncode(flight.FlightNumber);
                string safeAirlineName = WebUtility.HtmlEncode(flight.AirlineName);
                string safeOriginAirportName = WebUtility.HtmlEncode(flight.OriginAirportName);
                string safeOriginAirportCode = WebUtility.HtmlEncode(flight.OriginAirportCode);
                string safeDestinationAirportName = WebUtility.HtmlEncode(flight.DestinationAirportName);
                string safeDestinationAirportCode = WebUtility.HtmlEncode(flight.DestinationAirportCode);

                body.AppendLine($@"
                    <div style='border: 1px solid #e5e7eb; border-radius: 6px; padding: 14px; margin-bottom: 12px;'>
                        <p style='margin: 0 0 8px 0;'>
                            <strong>Vuelo {safeFlightNumber}</strong>
                        </p>

                        <p style='margin: 0 0 6px 0;'>
                            <strong>{safeOriginAirportCode} → {safeDestinationAirportCode}</strong>
                        </p>

                        <p style='margin: 0 0 6px 0;'>
                            {safeOriginAirportName} → {safeDestinationAirportName}
                        </p>

                        <p style='margin: 0 0 6px 0;'>
                            Aerolínea: {safeAirlineName}
                        </p>

                        <p style='margin: 0 0 6px 0;'>
                            Salida: {flight.DepartureDateTime:dd/MM/yyyy HH:mm}
                        </p>

                        <p style='margin: 0;'>
                            Llegada: {flight.ArrivalDateTime:dd/MM/yyyy HH:mm}
                        </p>
                    </div>");
            }

            return body.ToString();
        }
    }
}