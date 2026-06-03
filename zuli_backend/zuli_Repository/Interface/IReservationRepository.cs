using System;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IReservationRepository
    {
        Task<int> CreateReservation(ReservationEntity reservation);
        Task CreatePassengerReservation(PassengerReservationEntity pr);
        Task CreateBoardingPass(BoardingPassEntity boardingPass);
        
        Task<bool> PassengerExistsInFlights(
            IEnumerable<Guid> flightIds,
            string firstName,
            string firstLastName,
            string secondLastName,
            string birthDate,
            string passportCountry
        );
    }
}
