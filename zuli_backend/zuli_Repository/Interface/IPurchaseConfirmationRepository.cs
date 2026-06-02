using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IPurchaseConfirmationRepository
    {
        Task<PurchaseConfirmationEntity?> GetPurchaseConfirmationAsync(int reservationId);
    }
}