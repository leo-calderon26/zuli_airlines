using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class ReservationSearchRepository : DapperRepository, IReservationSearchRepository
    {
        public ReservationSearchRepository(DapperContext context) : base(context) { }

        public async Task<(List<ReservationSearchFlightEntity> Flights, int PassengerCount)> GetReservationDataAsync(
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
                var passengerCount = await multi.ReadSingleOrDefaultAsync<int>();

                if (!flights.Any())
                {
                    passengerCount = 0;
                }

                return (flights, passengerCount);
            });
        }
    }
}
