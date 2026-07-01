namespace zuli_Business.Interface
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string toEmail, string fullName, string activationLink);

        Task SendInvoiceEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            byte[] invoicePdf
        );

        Task SendPurchaseConfirmationEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            byte[] confirmationPdf
        );

        Task SendAdditionalBaggagePurchaseEmailAsync(
            string toEmail,
            string buyerName,
            string reservationCode,
            int additionalCheckedBaggage,
            int additionalCarryOn,
            decimal additionalBaggageTotal,
            decimal reservationTotal
        );

        Task SendCancellationRequestEmailAsync(
            string toEmail,
            string buyerName,
            string confirmationLink
        );
    }
}