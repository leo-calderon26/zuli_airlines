using zuli_business.DTO;

namespace zuli_business.Interface
{
    public interface IPurchaseConfirmationService
    {
        Task<PurchaseConfirmationPageDTO?> GetConfirmationPageAsync(Guid reservationId);
        Task<PurchaseConfirmationPageDTO> CompleteConfirmationAsync(Guid reservationId);
    }
}