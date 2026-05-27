using zuli_Business.DTO.PurchaseConfirmation;

namespace zuli_Data.Repositories.PurchaseConfirmation
{
    public interface IPurchaseConfirmationRepository
    {
        Task<PurchaseConfirmationPageDTO?> GetPurchaseConfirmationAsync(Guid reservationId);
    }
}