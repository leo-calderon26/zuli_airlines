using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IReservationRepository
    {
        Task<int> CreateReservation(ReservationEntity reservation, IUnitOfWork? uow = null);
        Task<int> CreatePassengerReservationsBulk(List<PassengerReservationEntity> pr, IUnitOfWork? uow = null);
        Task<int> CreateBoardingPassesBulk(List<BoardingPassEntity> boardingPasses, IUnitOfWork? uow = null);
        Task<bool> ReservationCodeExists(string reservationCode, IUnitOfWork? uow = null);

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
