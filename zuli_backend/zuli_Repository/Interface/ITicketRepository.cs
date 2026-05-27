using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface ITicketRepository
    {
        Task<ReservationEntity?> GetReservationByCode(string code);
        Task<IEnumerable<PassengerReservationEntity>> GetPassengerReservations(int reservationId);
        Task<PersonEntity?> GetPersonById(int personId);
    }
}
