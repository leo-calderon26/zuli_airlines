using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPurchaseConfirmationPdfService
    {
        byte[] GenerateInvoicePdf(PurchaseConfirmationPageDTO confirmation);
        byte[] GenerateConfirmationPdf(PurchaseConfirmationPageDTO confirmation);
    }
}