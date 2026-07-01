using System.Data;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class OutsideFlightRepository : DapperRepository, IOutsideFlightRepository
    {
        public OutsideFlightRepository(DapperContext context) : base(context)
        {
        }

        public async Task CreateOutsideFlightBulk(List<OutsideFlightEntity> outsideFlights)
        {
            await WithConnectionAsync(async (connection) =>
            {
                var table = new DataTable();
                table.Columns.Add("AirlineId", typeof(int));
                table.Columns.Add("ArrivalAirportCode", typeof(string));
                table.Columns.Add("ArrivalAirportName", typeof(string));
                table.Columns.Add("ArrivalAirportCity", typeof(string));
                table.Columns.Add("DepartureAirportCode", typeof(string));
                table.Columns.Add("DepartureAirportName", typeof(string));
                table.Columns.Add("DepartureAirportCity", typeof(string));
                table.Columns.Add("RealArrivalTime", typeof(TimeSpan));
                table.Columns.Add("RealDepartureTime", typeof(TimeSpan));
                table.Columns.Add("Frequency", typeof(int));
                table.Columns.Add("Duration", typeof(int));
                table.Columns.Add("CarryOnPrice", typeof(decimal));
                table.Columns.Add("CheckedPrice", typeof(decimal));
                table.Columns.Add("TouristPrice", typeof(decimal));
                table.Columns.Add("FirstClassPrice", typeof(decimal));
                
                foreach (var o in outsideFlights)
                {
                    table.Rows.Add(o.AirlineId, o.ArrivalAirportCode, o.ArrivalAirportName, o.ArrivalAirportCity, o.DepartureAirportCode, o.DepartureAirportName, o.DepartureAirportCity, o.RealArrivalTime, o.RealDepartureTime, o.Frequency, o.DurationOnSeconds, o.CarryOnPrice, o.CheckedPrice, o.TouristPrice, o.FirstClassPrice);
                }
                var parameters = new DynamicParameters();
                parameters.Add("OutsideFlightRoutes", table.AsTableValuedParameter("dbo.OutsideFlightRouteBulkType"));

                await connection.ExecuteAsync(
                    "dbo.sp_BulkOutsideFlightRoutes",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            });
        }

        public async Task UpdateOutsideFlightsStatus() {
            await WithConnectionAsync(async (connection) =>
            {
                var sql = @"
                    UPDATE FlightRoute SET Status = 'Deshabilitada' WHERE AirlineId <> 1";

                return await connection.ExecuteScalarAsync(sql);
            });
        }
    }
}
