using zuli_Data.Enums;

namespace zuli_Repository.Interface
{
    public interface IReservationCancellationRepository
    {
        Task<(string? BuyerEmail, string? BuyerName, int ReservationStatusId, int ReservationId)?>
            GetCancellationInfoAsync(string reservationCode);

        Task<int> SetCancellationTokenAsync(
            string reservationCode,
            string tokenHash,
            DateTime requestedAt,
            DateTime expiresAt
        );

        Task<CancellationConfirmationResult> ConfirmCancellationAsync(
            string tokenHash
        );
    }
}