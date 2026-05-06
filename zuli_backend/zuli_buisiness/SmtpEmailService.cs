using System.Net;
using System.Net.Mail;
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