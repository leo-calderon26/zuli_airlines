using zuli_Data;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IReservationRepository
    {
        Task<int> CreateReservation(ReservationEntity reservation);
        Task<int> CreatePassengerReservationsBulk(List<PassengerReservationEntity> pr);
        Task<int> CreateBoardingPassesBulk(List<BoardingPassEntity> boardingPasses);
        Task<HashSet<string>> GetAllReservationCodes();

        Task<bool> PassengerExistsInFlights(
            IEnumerable<Guid> flightIds,
            string firstName,
            string firstLastName,
            string secondLastName,
            string birthDate,
            string passportCountry
        );

        Task<Dictionary<int, bool>> PassengersExistInFlights(
            IEnumerable<Guid> flightIds,
            List<PassengerCheckInfo> passengers
        );
    }
}
