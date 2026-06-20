using Dapper;
using zuli_Data;
using zuli_Data.Entities.Filters;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FilterOptionsRepository : IFilterOptionsRepository
    {
        private readonly DapperContext _context;
        public FilterOptionsRepository(DapperContext context)
            => _context = context;

        public async Task<FilterOptionsEntity> GetFilterOptionsAsync()
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                -- se trae solo los años de los vuelos
                SELECT DISTINCT YEAR(FlightDate) AS Year FROM Flight ORDER BY Year DESC;
                -- se trae solo los codigos de los aeropuertos de origen
                SELECT DISTINCT a.AirportCode
                FROM FlightRoute fr 
                JOIN Airport a ON fr.DepartureAirport = a.AirportCode
                ORDER BY a.AirportCode;
                -- se trae solo los codigos de los aeropuertos de destino
                SELECT DISTINCT a.AirportCode
                FROM FlightRoute fr
                JOIN Airport a ON fr.ArrivalAirport = a.AirportCode
                ORDER BY a.AirportCode;
                -- se trae solo los codigos de las aerolineas
                SELECT DISTINCT AirlineId AS AirlineCode, AirlineName 
                FROM Airline ORDER BY AirlineName;
            ";
            using var multi = await connection.QueryMultipleAsync(sql);
            var years = (await multi.ReadAsync<int>()).ToList();
            var origins = (await multi.ReadAsync<AirportOptionEntity>()).ToList();
            var destinations = (await multi.ReadAsync<AirportOptionEntity>()).ToList();
            var airlines = (await multi.ReadAsync<AirlineOptionEntity>()).ToList();
            return new FilterOptionsEntity
            {
                Years = years,
                Origins = origins,
                Destinations = destinations,
                Airlines = airlines,
                Classes = new()
                {
                    new()
                    {
                        Value = "Economy",
                        Label = "Turista" 
                    },
                    new()
                    {
                        Value = "FirstClass",
                        Label = "Primera Clase" 
                    },
                }
            };
        }
        
    }
}

