using Dapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;
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
                        INSERT INTO Aircraft (aircraftId, model, weight, numberEconomyClassRows, numberSeatingRowsEconomy, 
                                            numberFirstClassRows, numberSeatingRowsFirst)
                        VALUES (NEWID(),@Model, @Weight, @NumberEconomyClassRows, @NumberSeatingRowsEconomy, 
                                @NumberFirstClassRows, @NumberSeatingRowsFirst)";

            return await connection.ExecuteScalarAsync<int>(insertSql, new
            {
                model = aircraft.model,
                weight = aircraft.weight,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
            }); 
        }

        public async Task<bool> AlreadyExistByModel(string model)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Aircraft WHERE model = @model";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { model });
            return count > 0;
        }
        public async Task<IEnumerable<AircraftEntity>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * From Aircraft";
            var arrayAircraf = (await connection.QueryAsync<AircraftEntity>(sql)).ToList();
            return arrayAircraf;
        }
    }   
}
