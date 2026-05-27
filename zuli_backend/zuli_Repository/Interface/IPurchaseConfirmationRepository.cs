using zuli_Business.DTO;

namespace zuli_Data.Repositories.Interface
{
    public interface IPurchaseConfirmationRepository
    {
        Task<PurchaseConfirmationPageDTO?> GetPurchaseConfirmationAsync(Guid reservationId);
    }
}