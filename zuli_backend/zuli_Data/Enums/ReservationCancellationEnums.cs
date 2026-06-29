namespace zuli_Data.Enums
{
    public enum ReservationStatus
    {
        Active = 1,
        Cancelled = 2
    }

    public enum CancellationConfirmationResult
    {
        InvalidOrExpiredOrUsed = 0,
        Confirmed = 1
    }
}