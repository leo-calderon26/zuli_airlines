using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FlightsReportRepository : IFlightsReportRepository
    {
        private readonly DapperContext _context;

        // Usamos exactamente el mismo constructor que usás en Airport
        public FlightsReportRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FlightsReportEntity>> GetFlightsReportAsync()
        {
            using var connection = _context.CreateConnection();

            // La consulta limpia basada 100% en lo que te funcionó en SSMS
            var sql = @"
                SELECT 
                    f.FlightDate AS Fecha,
                    f.RealDepartureAirport AS Origen,
                    f.RealArrivalAirport AS Destino,
                    f.FlightRouteId AS NumeroVuelo,
                    a.AirlineName AS Aerolinea,
                    
                    -- Lógica simple: si la fila tiene la clase, sumamos 1, si no 0
                    CASE WHEN r.FlightClass = 'FirstClass' THEN 1 ELSE 0 END AS PasajerosPrimera,
                    CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END AS PasajerosEconomica,
                    
                    -- Dinero crudo por fila
                    ISNULL(r.TotalPayment, 0.00) AS VentaPasajeros,
                    (ISNULL(f.CarryOnPrice, 0.00) + ISNULL(f.CheckedPrice, 0.00)) AS VentaEquipaje,
                    (ISNULL(r.TotalPayment, 0.00) + ISNULL(f.CarryOnPrice, 0.00) + ISNULL(f.CheckedPrice, 0.00)) AS TotalVenta
                FROM dbo.Flight f
                INNER JOIN dbo.FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
                INNER JOIN dbo.Airline a ON fr.AirlineId = a.AirlineId
                LEFT JOIN dbo.BoardingPass bp ON f.Id = bp.FlightId
                LEFT JOIN dbo.Reservation r ON bp.ReservationCode = r.ReservationCode;";


            var report = await connection.QueryAsync<FlightsReportEntity>(sql);

            return report;
        }
    }
}