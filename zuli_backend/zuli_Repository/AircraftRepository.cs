using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using Dapper;
namespace zuli_Repository
{
    public class AircraftRepository : IAircraftRepository
    {
        // Inyeccion de dependencias de la capa Data
        private readonly DapperContext _context;

        public AircraftRepository(DapperContext context) 
        {
            _context = context;
            
        }

        public async Task<int> CreateAircraft(AircraftEntity aircraft)
        {
            using var connection = _context.CreateConnection();
            // Insertar la nueva aeronave
            var insertSql = @"
                        INSERT INTO Aircraft (AircraftId, numberEconomyClassRows, numberSeatingRowsEconomy, 
                                            numberFirstClassRows, numberSeatingRowsFirst, model, weight)
                        VALUES (@AircraftId, @NumberEconomyClassRows, @NumberSeatingRowsEconomy, 
                                @NumberFirstClassRows, @NumberSeatingRowsFirst, @Model, @Weight)";

            return await connection.ExecuteScalarAsync<int>(insertSql, new
            {
                aircraftId = aircraft.AircraftId,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
                model = aircraft.model,
                weight = aircraft.weight
            }); 
        }

        /// <summary>
        /// Varifica que una aeronave no exista en la base de datos
        /// </summary>
        /// <param name="aircraftId">Id a comparar</param>
        /// <returns>
        /// <c>true</c> si la aeronave existe; en caso contrario, <c>false</c
        /// </returns>
        public async Task<bool> AlreadyExist(int aircraftId)
        {
            // Con dapper se recomienda hacer una conexion cada vez
            using var connection = _context.CreateConnection();
            // Verificar si existe una aeronave ya con el mismo id
            var sql = "SELECT COUNT(1) FROM Aircraft WHERE AircraftId = @AircraftId";
            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { aircraftId = aircraftId }
            );
            return count > 0;
        }

    }   
}
