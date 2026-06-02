using System.Net;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string BuildActivationEmailBody(
            string fullName,
            string activationLink)
        {
            string safeFullName = WebUtility.HtmlEncode(fullName);
            string safeActivationLink = WebUtility.HtmlEncode(activationLink);

            string content = $@"
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
                </p>";

            return BuildHtmlTemplate(content);
        }

        public string BuildInvoiceEmailBody(
            string buyerName,
            string reservationCode)
        {
            string safeBuyerName = WebUtility.HtmlEncode(buyerName);
            string safeReservationCode = WebUtility.HtmlEncode(reservationCode);

            string content = $@"
                <h3>Factura de compra</h3>

                <p>Hola {safeBuyerName},</p>

                <p>
                    Adjuntamos en PDF la factura correspondiente a su compra.
                </p>

                <p>
                    <strong>Código de reserva:</strong> {safeReservationCode}
                </p>";

            return BuildHtmlTemplate(content);
        }

        public string BuildPurchaseConfirmationEmailBody(
            string buyerName,
            string reservationCode)
        {
            string safeBuyerName = WebUtility.HtmlEncode(buyerName);
            string safeReservationCode = WebUtility.HtmlEncode(reservationCode);

            string content = $@"
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
                </p>";

            return BuildHtmlTemplate(content);
        }

        private static string BuildHtmlTemplate(string content)
        {
            return $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #1f2937; background-color: #f3f3f3; padding: 24px;'>
                        <div style='max-width: 650px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                            <div style='background-color: #711717; padding: 20px; text-align: center;'>
                                <h2 style='color: #ffffff; margin: 0;'>Zuli Airlines</h2>
                            </div>

                            <div style='padding: 28px;'>
                                {content}

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