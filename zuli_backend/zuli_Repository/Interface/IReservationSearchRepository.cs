using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IReservationSearchRepository
    {
        Task<(List<ReservationSearchFlightEntity> Flights, List<ReservationSearchPassengerEntity> Passengers)> GetReservationDataAsync(string reservationCode, string lastName);
    }
}