using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettingsDTO _emailSettings;
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly IEmailTemplateService _emailTemplateService;

        public SmtpEmailService(
            IConfiguration configuration,
            ILogger<SmtpEmailService> logger,
            IEmailTemplateService emailTemplateService)
        {
            _logger = logger;
            _emailTemplateService = emailTemplateService;

            _emailSettings = configuration
                .GetSection("EmailSettings")
                .Get<EmailSettingsDTO>()
                ?? throw new InvalidOperationException("EmailSettings is not configured.");

            ValidateEmailSettings();
        }

        public async Task SendActivationEmailAsync(
            string toEmail,
            string fullName,
            string activationLink)
        {
            string body = _emailTemplateService.BuildActivationEmailBody(
                fullName,
                activationLink
            );

            await SendEmailAsync(
                toEmail,
                "Activación de cuenta - Zuli Airlines",
                body
            );

            _logger.LogInformation(
                "Activation email sent to {Email}",
                toEmail
            );
        }

        public async Task SendInvoiceEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            byte[] invoicePdf)
        {
            string body = _emailTemplateService.BuildInvoiceEmailBody(
                buyerName,
                reservationCode
            );

            await SendEmailWithPdfAttachmentAsync(
                toEmail,
                $"Factura de compra - Reserva {reservationCode}",
                body,
                invoicePdf,
                $"Factura-{reservationCode}.pdf"
            );

            _logger.LogInformation(
                "Invoice email sent to {Email} for reservation {ReservationCode}",
                toEmail,
                reservationCode
            );
        }

        public async Task SendPurchaseConfirmationEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            byte[] confirmationPdf)
        {
            string body = _emailTemplateService.BuildPurchaseConfirmationEmailBody(
                buyerName,
                reservationCode
            );

            await SendEmailWithPdfAttachmentAsync(
                toEmail,
                $"Confirmación de compra - Reserva {reservationCode}",
                body,
                confirmationPdf,
                $"Confirmacion-{reservationCode}.pdf"
            );

            _logger.LogInformation(
                "Purchase confirmation email sent to {Email} for reservation {ReservationCode}",
                toEmail,
                reservationCode
            );
        }

        public async Task SendAdditionalBaggagePurchaseEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            int additionalCheckedBaggage,
            int additionalCarryOn,
            decimal additionalBaggageTotal,
            decimal reservationTotal)
        {
            string body = _emailTemplateService.BuildAdditionalBaggagePurchaseEmailBody(
                buyerName,
                reservationCode,
                additionalCheckedBaggage,
                additionalCarryOn,
                additionalBaggageTotal,
                reservationTotal
            );

            await SendEmailAsync(
                toEmail,
                $"Compra de equipaje adicional - Reserva {reservationCode}",
                body
            );

            _logger.LogInformation(
                "Additional baggage purchase email sent to {Email} for reservation {ReservationCode}",
                toEmail,
                reservationCode
            );
        }

        public async Task SendCancellationRequestEmailAsync(
            string toEmail,
            string buyerName,
            string confirmationLink)
        {
            string body = _emailTemplateService.BuildCancellationRequestEmailBody(
                buyerName,
                confirmationLink
            );

            await SendEmailAsync(
                toEmail,
                "Solicitud de cancelación de reserva - Zuli Airlines",
                body
            );

            _logger.LogInformation(
                "Cancellation request email sent to {Email}",
                toEmail
            );
        }

        private async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            using var message = CreateMailMessage(
                toEmail,
                subject,
                body
            );

            await SendMessageAsync(message);
        }

        private async Task SendEmailWithPdfAttachmentAsync(
            string toEmail,
            string subject,
            string body,
            byte[] pdfFile,
            string fileName)
        {
            using var message = CreateMailMessage(
                toEmail,
                subject,
                body
            );

            AddPdfAttachment(
                message,
                pdfFile,
                fileName
            );

            await SendMessageAsync(message);
        }

        private MailMessage CreateMailMessage(
            string toEmail,
            string subject,
            string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(
                    _emailSettings.FromEmail,
                    _emailSettings.FromName
                ),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            return message;
        }

        private static void AddPdfAttachment(
            MailMessage message,
            byte[] pdfFile,
            string fileName)
        {
            var pdfStream = new MemoryStream(pdfFile);

            var attachment = new Attachment(
                pdfStream,
                fileName,
                MediaTypeNames.Application.Pdf
            );

            message.Attachments.Add(attachment);
        }

        private async Task SendMessageAsync(MailMessage message)
        {
            using var smtpClient = CreateSmtpClient();
            await smtpClient.SendMailAsync(message);
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(
                _emailSettings.SmtpHost,
                _emailSettings.SmtpPort
            )
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _emailSettings.SmtpUser,
                    _emailSettings.SmtpPassword
                )
            };
        }

        private void ValidateEmailSettings()
        {
            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost))
            {
                throw new InvalidOperationException("EmailSettings:SmtpHost is not configured.");
            }

            if (_emailSettings.SmtpPort <= 0)
            {
                throw new InvalidOperationException("EmailSettings:SmtpPort is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpUser))
            {
                throw new InvalidOperationException("EmailSettings:SmtpUser is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpPassword))
            {
                throw new InvalidOperationException("EmailSettings:SmtpPassword is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_emailSettings.FromEmail))
            {
                throw new InvalidOperationException("EmailSettings:FromEmail is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_emailSettings.FromName))
            {
                throw new InvalidOperationException("EmailSettings:FromName is not configured.");
            }
        }
    }
}