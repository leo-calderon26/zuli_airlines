using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPurchaseEmailService
    {
        Task SendPurchaseEmailsAsync(string reservationCode, CancellationToken cancellationToken = default);
        Task SendPurchaseEmailsAsync(PurchaseConfirmationPageDTO confirmation, CancellationToken cancellationToken = default);
    }
}
