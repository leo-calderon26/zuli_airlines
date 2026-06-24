using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using zuli_Data;
using zuli_Data.Entities.Reports;
using zuli_Data.Entities.Filters;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FlightsReportRepository : IFlightsReportRepository
    {
        private readonly DapperContext _context;

        public FlightsReportRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FlightsReportEntity>> GetFlightsReportAsync(FlightsReportFilterEntity filters)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
            SELECT 
                f.FlightDate AS Fecha,
                f.RealDepartureAirport AS Origen,
                f.RealArrivalAirport AS Destino,
                f.FlightRouteId AS NumeroVuelo,
                a.AirlineName AS Aerolinea,
                
                SUM(CASE WHEN r.FlightClass = 'Primera Clase' THEN 1 ELSE 0 END) AS PasajerosPrimera,
                SUM(CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END) AS PasajerosEconomica,
                
                (SUM(CASE WHEN r.FlightClass = 'Primera Clase' THEN 1 ELSE 0 END) * f.FirstClassPrice) +
                (SUM(CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END) * f.TouristPrice) AS VentaPasajeros,
                
                SUM(CASE WHEN bp.FlightId IS NOT NULL THEN (ISNULL(f.CarryOnPrice, 0.00) + ISNULL(f.CheckedPrice, 0.00)) ELSE 0.00 END) AS VentaEquipaje,
                
                ((SUM(CASE WHEN r.FlightClass = 'Primera Clase' THEN 1 ELSE 0 END) * f.FirstClassPrice) +
                 SUM(CASE WHEN r.FlightClass = 'Turista' OR r.FlightClass = 'Economy' THEN 1 ELSE 0 END) * f.TouristPrice) +
                SUM(CASE WHEN bp.FlightId IS NOT NULL THEN (ISNULL(f.CarryOnPrice, 0.00) + ISNULL(f.CheckedPrice, 0.00)) ELSE 0.00 END) AS TotalVenta
                
            FROM dbo.Flight f
            INNER JOIN dbo.FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
            INNER JOIN dbo.Airline a ON fr.AirlineId = a.AirlineId
            LEFT JOIN dbo.BoardingPass bp ON f.Id = bp.FlightId
            LEFT JOIN dbo.Reservation r ON bp.reservationCode = r.reservationCode
            
            WHERE 
                (@FromDate IS NULL OR f.FlightDate >= @FromDate)
                AND (@ToDate IS NULL OR f.FlightDate <= @ToDate)
                AND (@Origin IS NULL OR @Origin = '' OR f.RealDepartureAirport = @Origin)
                AND (@Destination IS NULL OR @Destination = '' OR f.RealArrivalAirport = @Destination)
                AND (@FlightClass IS NULL OR @FlightClass = '' OR r.FlightClass = @FlightClass)
            
            GROUP BY 
                f.Id, 
                f.FlightDate,
                f.RealDepartureAirport,
                f.RealArrivalAirport,
                f.FlightRouteId,
                a.AirlineName,
                f.FirstClassPrice,  
                f.TouristPrice;
            ";

            var report = await connection.QueryAsync<FlightsReportEntity>(sql, filters);

            return report;
        }
    }
}