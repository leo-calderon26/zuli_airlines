namespace zuli_Business.Interface
{
	public interface IEmailTemplateService
	{
		string BuildActivationEmailBody(string fullName, string activationLink);

		string BuildInvoiceEmailBody(
			string buyerName,
			string reservationCode
		);

		string BuildInvoiceEmailBody(
			string buyerName,
			string reservationCode,
			string invoiceBody
		);

		string BuildPurchaseConfirmationEmailBody(
			string buyerName,
			string reservationCode
		);
	}
}