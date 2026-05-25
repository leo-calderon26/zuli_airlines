namespace zuli_Business.Interface
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string toEmail, 
            string fullName, string activationLink);

        Task SendInvoiceEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            string invoiceBody
        );

        Task SendPurchaseConfirmationEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            string confirmationBody
        );
    }
}