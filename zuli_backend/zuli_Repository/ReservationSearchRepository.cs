using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class ReservationSearchRepository : DapperRepository, IReservationSearchRepository
    {
        public ReservationSearchRepository(DapperContext context) : base(context) { }

        public async Task<(List<ReservationSearchFlightEntity> Flights, List<ReservationSearchPassengerEntity> Passengers)> GetReservationDataAsync(
            string reservationCode, 
            string lastNameSearch)
        {
            return await WithConnectionAsync(async (connection) =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetReservationSearchData", 
                    new { ReservationCode = reservationCode, LastNameSearch = lastNameSearch },
                    commandType: CommandType.StoredProcedure);

                var flights = (await multi.ReadAsync<ReservationSearchFlightEntity>()).ToList();
                var passengers = (await multi.ReadAsync<ReservationSearchPassengerEntity>()).ToList();

                if (!flights.Any())
                {
                    passengers = new List<ReservationSearchPassengerEntity>();
                }

                return (flights, passengers);
            });
        }
    }
}
