using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPurchaseConfirmationService
    {
        Task<PurchaseConfirmationPageDTO?> GetConfirmationPageAsync(int reservationId);

        Task<PurchaseConfirmationPageDTO> CompleteConfirmationAsync(int reservationId);
    }
}