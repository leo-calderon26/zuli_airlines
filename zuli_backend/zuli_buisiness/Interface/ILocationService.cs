using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface ILocationService
    {
        Task<IEnumerable<CountryDTO>> GetCountriesAsync();
        Task<IEnumerable<CityDTO>> GetCitiesByCountryIdAsync(int countryId);
    }
}