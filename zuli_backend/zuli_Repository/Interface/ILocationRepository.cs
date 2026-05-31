using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface ILocationRepository
    {
        Task<IEnumerable<CountryEntity>> GetCountriesAsync();
        Task<IEnumerable<CityEntity>> GetCitiesByCountryIdAsync(int countryId);
    }
}