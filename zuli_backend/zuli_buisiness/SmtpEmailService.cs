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

        public SmtpEmailService(
            IConfiguration configuration,
            ILogger<SmtpEmailService> logger)
        {
            _logger = logger;

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
            using var message = new MailMessage();

            message.From = new MailAddress(
                _emailSettings.FromEmail,
                _emailSettings.FromName
            );

            message.To.Add(toEmail);
            message.Subject = "Activación de cuenta - Zuli Airlines";
            message.IsBodyHtml = true;
            message.Body = BuildActivationEmailBody(fullName, activationLink);

            using var smtpClient = new SmtpClient(
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

            await smtpClient.SendMailAsync(message);

            _logger.LogInformation(
                "Activation email sent to {Email}",
                toEmail
            );
        }
        public async Task SendInvoiceEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            string invoiceBody)
        {
            string safeBuyerName = WebUtility.HtmlEncode(buyerName);
            string safeReservationCode = WebUtility.HtmlEncode(reservationCode);

            using var message = new MailMessage();

            message.From = new MailAddress(
                _emailSettings.FromEmail,
                _emailSettings.FromName
            );

            message.To.Add(toEmail);
            message.Subject = $"Factura de compra - Reserva {reservationCode}";
            message.IsBodyHtml = true;
            message.Body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #1f2937; background-color: #f3f3f3; padding: 24px;'>
                        <div style='max-width: 650px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                            <div style='background-color: #711717; padding: 20px; text-align: center;'>
                                <h2 style='color: #ffffff; margin: 0;'>Zuli Airlines</h2>
                            </div>

                            <div style='padding: 28px;'>
                                <h3>Factura de compra</h3>

                                <p>Hola {safeBuyerName},</p>

                                <p>
                                    Adjuntamos el detalle de la factura correspondiente a su compra.
                                </p>

                                <p>
                                    <strong>Código de reserva:</strong> {safeReservationCode}
                                </p>

                                {invoiceBody}

                                <br />

                                <p>Atentamente,</p>
                                <p><strong>Zuli Airlines</strong></p>
                            </div>
                        </div>
                    </body>
                </html>";

            using var smtpClient = new SmtpClient(
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

            await smtpClient.SendMailAsync(message);

            _logger.LogInformation(
                "Invoice email sent to {Email} for reservation {ReservationCode}",
                toEmail,
                reservationCode
            );
        }
        public async Task SendInvoiceEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            byte[] invoicePdf)
        {
            string safeBuyerName = WebUtility.HtmlEncode(buyerName);
            string safeReservationCode = WebUtility.HtmlEncode(reservationCode);

            using var message = new MailMessage();

            message.From = new MailAddress(
                _emailSettings.FromEmail,
                _emailSettings.FromName
            );

            message.To.Add(toEmail);
            message.Subject = $"Factura de compra - Reserva {reservationCode}";
            message.IsBodyHtml = true;
            message.Body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #1f2937; background-color: #f3f3f3; padding: 24px;'>
                        <div style='max-width: 650px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                            <div style='background-color: #711717; padding: 20px; text-align: center;'>
                                <h2 style='color: #ffffff; margin: 0;'>Zuli Airlines</h2>
                            </div>

                            <div style='padding: 28px;'>
                                <h3>Factura de compra</h3>

                                <p>Hola {safeBuyerName},</p>

                                <p>
                                    Adjuntamos en PDF la factura correspondiente a su compra.
                                </p>

                                <p>
                                    <strong>Código de reserva:</strong> {safeReservationCode}
                                </p>

                                <br />

                                <p>Atentamente,</p>
                                <p><strong>Zuli Airlines</strong></p>
                            </div>
                        </div>
                    </body>
                </html>";

            using var invoiceStream = new MemoryStream(invoicePdf);

            message.Attachments.Add(
                new Attachment(
                    invoiceStream,
                    $"Factura-{reservationCode}.pdf",
                    MediaTypeNames.Application.Pdf
                )
            );

            using var smtpClient = new SmtpClient(
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

            await smtpClient.SendMailAsync(message);

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
            string safeBuyerName = WebUtility.HtmlEncode(buyerName);
            string safeReservationCode = WebUtility.HtmlEncode(reservationCode);

            using var message = new MailMessage();

            message.From = new MailAddress(
                _emailSettings.FromEmail,
                _emailSettings.FromName
            );

            message.To.Add(toEmail);
            message.Subject = $"Confirmación de compra - Reserva {reservationCode}";
            message.IsBodyHtml = true;
            message.Body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #1f2937; background-color: #f3f3f3; padding: 24px;'>
                        <div style='max-width: 650px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                            <div style='background-color: #711717; padding: 20px; text-align: center;'>
                                <h2 style='color: #ffffff; margin: 0;'>Zuli Airlines</h2>
                            </div>

                            <div style='padding: 28px;'>
                                <h3>Confirmación de compra</h3>

                                <p>Hola {safeBuyerName},</p>

                                <p>
                                    Felicitaciones, su compra fue confirmada exitosamente.
                                </p>

                                <p>
                                    Adjuntamos en PDF la confirmación de compra con la información de pasajeros e itinerario.
                                </p>

                                <p>
                                    <strong>Código de reserva:</strong> {safeReservationCode}
                                </p>

                                <br />

                                <p>Atentamente,</p>
                                <p><strong>Zuli Airlines</strong></p>
                            </div>
                        </div>
                    </body>
                </html>";

            using var confirmationStream = new MemoryStream(confirmationPdf);

            message.Attachments.Add(
                new Attachment(
                    confirmationStream,
                    $"Confirmacion-{reservationCode}.pdf",
                    MediaTypeNames.Application.Pdf
                )
            );

            using var smtpClient = new SmtpClient(
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

            await smtpClient.SendMailAsync(message);

            _logger.LogInformation(
                "Purchase confirmation email sent to {Email} for reservation {ReservationCode}",
                toEmail,
                reservationCode
            );
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

        private static string BuildActivationEmailBody(string fullName, string activationLink)
        {
            string safeFullName = WebUtility.HtmlEncode(fullName);
            string safeActivationLink = WebUtility.HtmlEncode(activationLink);

            return $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #1f2937; background-color: #f3f3f3; padding: 24px;'>
                        <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                            <div style='background-color: #711717; padding: 20px; text-align: center;'>
                                <h2 style='color: #ffffff; margin: 0;'>Zuli Airlines</h2>
                            </div>

                            <div style='padding: 28px;'>
                                <h3>Activación de cuenta</h3>

                                <p>Hola {safeFullName},</p>

                                <p>
                                    Se ha creado una cuenta para usted en el sistema administrativo de Zuli Airlines.
                                </p>

                                <p>
                                    Para activar su cuenta, ingrese al siguiente enlace y configure su contraseña.
                                </p>

                                <p style='text-align: center; margin: 32px 0;'>
                                    <a href='{safeActivationLink}'
                                       style='display: inline-block; padding: 12px 24px; background-color: #711717; color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold;'>
                                        Activar cuenta
                                    </a>
                                </p>

                                <p>
                                    Si el botón no funciona, copie y pegue este enlace en su navegador:
                                </p>

                                <p style='word-break: break-all; color: #711717;'>
                                    {safeActivationLink}
                                </p>

                                <br />

                                <p>Atentamente,</p>
                                <p><strong>Zuli Airlines</strong></p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}