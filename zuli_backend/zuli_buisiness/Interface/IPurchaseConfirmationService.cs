using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IPurchaseConfirmationService
    {
        Task<PurchaseConfirmationPageDTO?> GetConfirmationPageAsync(string reservationCode);
        
        Task<PurchaseConfirmationPageDTO> CompleteConfirmationAsync(string reservationCode);
    }
}