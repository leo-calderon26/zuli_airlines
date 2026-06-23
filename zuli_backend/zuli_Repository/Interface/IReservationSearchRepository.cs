using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IReservationSearchRepository
    {
        Task<(List<ReservationSearchFlightEntity> Flights, int PassengerCount)> GetReservationDataAsync(string reservationCode, string lastName);
    }
}