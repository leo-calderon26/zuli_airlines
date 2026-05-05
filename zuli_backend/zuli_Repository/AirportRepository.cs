using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using Dapper;

namespace zuli_Repository
{
    public class AirportRepository : IAirportRepository
    {
        private readonly DapperContext _context;

        public AirportRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<int> CreateAirport(AirportEntity airport)
        {
            using var connection = _context.CreateConnection();

            var insertSql = @"
                        INSERT INTO Airport (AirportCode, Name, Country, City, AdminId)
                        VALUES (@AirportCode, @Name, @Country, @City, @AdminId)";

            return await connection.ExecuteScalarAsync<int>(insertSql, new
            {
                airportCode = airport.AirportCode,
                name = airport.Name,
                country = airport.Country,
                city = airport.City,
                adminId = airport.AdminId
            });
        }

        /// <summary>
        /// Verifica que un aeropuerto no exista en la base de datos
        /// </summary>
        /// <param name="airportCode">Codigo a comparar</param>
        /// <returns>
        /// <c>true</c> si el aeropuerto existe; en caso contrario, <c>false</c>
        /// </returns>
        public async Task<bool> AlreadyExist(string airportCode)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Airport WHERE AirportCode = @airportCode";
            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { airportCode = airportCode }
            );

            return count > 0;
        }
         public async Task<IEnumerable<AirportEntity>> SearchAirportsByTerm(string searchTerm)
        {
            using var connection = _context.CreateConnection();

            var parameters = new 
            { 
                Term = $"%{searchTerm}%",
                ExactTerm = $"{searchTerm}%" 
            };

            var sql = @"
            SELECT TOP 5 AirportCode, Name, Country, City, AdminId
            FROM (
                SELECT AirportCode, Name, Country, City, AdminId, 1 AS Priority
                FROM Airport 
                WHERE AirportCode LIKE @ExactTerm
        
                UNION ALL
        
                SELECT AirportCode, Name, Country, City, AdminId, 2 AS Priority
                FROM Airport 
                WHERE City LIKE @ExactTerm AND AirportCode NOT LIKE @ExactTerm
        
                UNION ALL
        
                SELECT AirportCode, Name, Country, City, AdminId, 3 AS Priority
                FROM Airport 
                WHERE (AirportCode LIKE @Term OR City LIKE @Term OR Country LIKE @Term)
                  AND AirportCode NOT LIKE @ExactTerm AND City NOT LIKE @ExactTerm
            ) AS Resultados
            ORDER BY Priority, City ASC";


            var airports = await connection.QueryAsync<AirportEntity>(sql, parameters);
            return airports;
        }

    }
}