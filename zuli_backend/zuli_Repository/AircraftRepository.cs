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
                    INSERT INTO Aircraft (aircraftId, model, weight, baggageCapacity, adminId,numberEconomyClassRows, numberSeatingRowsEconomy, 
                                numberFirstClassRows, numberSeatingRowsFirst)
                    VALUES (NEWID(),@Model, @Weight, @BaggageCapacity, @AdminId, @NumberEconomyClassRows, @NumberSeatingRowsEconomy, 
                        @NumberFirstClassRows, @NumberSeatingRowsFirst)";

            return await connection.ExecuteAsync(insertSql, new
            {
                adminId = aircraft.AdminId,
                model = aircraft.model,
                weight = aircraft.weight,
                baggageCapacity = aircraft.baggageCapacity,
                numberEconomyClassRows = aircraft.numberEconomyClassRows,
                numberSeatingRowsEconomy = aircraft.numberSeatingRowsEconomy,
                numberFirstClassRows = aircraft.numberFirstClassRows,
                numberSeatingRowsFirst = aircraft.numberSeatingRowsFirst,
            });
        }

        public async Task<IEnumerable<AircraftEntity>> GetAll()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * From Aircraft";
            var arrayAircraf = (await connection.QueryAsync<AircraftEntity>(sql)).ToList();
            return arrayAircraf;
        }

        public async Task<(IEnumerable<AircraftEntity> aircrafts, int totalCount)> GetAircraftsPaginated(int pageNumber, int pageSize)
        {
            using var connection = _context.CreateConnection();

            // Obtener el total de registros
            var countSql = "SELECT COUNT(1) FROM Aircraft";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            // Calcular el offset
            var offset = (pageNumber - 1) * pageSize;

            // Obtener los registros paginados
            var sql = @"
                SELECT * FROM Aircraft 
                ORDER BY aircraftId
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var aircrafts = (await connection.QueryAsync<AircraftEntity>(sql, new { Offset = offset, PageSize = pageSize })).ToList();
            return (aircrafts, totalCount);
        }

        public async Task<AircraftEntity?> GetById(Guid aircraftId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Aircraft WHERE aircraftId = @aircraftId";
            return await connection.QuerySingleOrDefaultAsync<AircraftEntity>(sql, new { aircraftId });
        }

        public async Task UpdateAircraftAsync(AircraftEntity aircraft)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                UPDATE Aircraft
                SET
                    model = @model,
                    weight = @weight,
                    baggageCapacity = @baggageCapacity,
                    numberEconomyClassRows = @numberEconomyClassRows,
                    numberSeatingRowsEconomy = @numberSeatingRowsEconomy,
                    numberFirstClassRows = @numberFirstClassRows,
                    numberSeatingRowsFirst = @numberSeatingRowsFirst
                WHERE aircraftId = @aircraftId";

            await connection.ExecuteAsync(sql, new
            {
                aircraft.aircraftId,
                aircraft.model,
                aircraft.weight,
                aircraft.baggageCapacity,
                aircraft.numberEconomyClassRows,
                aircraft.numberSeatingRowsEconomy,
                aircraft.numberFirstClassRows,
                aircraft.numberSeatingRowsFirst,
            });
        }

        public async Task DeleteAircraft(Guid aircraftId)
        {
            using var connection = _context.CreateConnection();

            var sql = "dbo.sp_HandleAircraftDeletion";

            await connection.ExecuteScalarAsync(sql, new
            {
                selectedAircraftToDelete = aircraftId
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> IsAdmin(Guid userId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT CASE WHEN EXISTS (
            SELECT 1
            FROM [User]
            WHERE UserId = @UserId AND UserRole = 'Administrator'
            ) THEN 1 ELSE 0 END";
            return await connection.ExecuteScalarAsync<bool>(sql, new { UserId = userId });
        }
    }
}
