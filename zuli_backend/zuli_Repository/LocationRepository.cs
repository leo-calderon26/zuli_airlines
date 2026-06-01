using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DapperContext _context;

        public LocationRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<CountryEntity>> GetCountriesAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT Id, CountryName FROM Country ORDER BY CountryName ASC";
            return await connection.QueryAsync<CountryEntity>(sql);
        }

        public async Task<IEnumerable<CityEntity>> GetCitiesByCountryIdAsync(int countryId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT Id, CountryId, CityName FROM City WHERE CountryId = @CountryId ORDER BY CityName ASC";
            return await connection.QueryAsync<CityEntity>(sql, new { CountryId = countryId });
        }
    }
}