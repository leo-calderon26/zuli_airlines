using MapsterMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDTO>> GetCountriesAsync()
        {
            var countries = await _repository.GetCountriesAsync();
            return _mapper.Map<IEnumerable<CountryDTO>>(countries);
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesByCountryIdAsync(int countryId)
        {
            var cities = await _repository.GetCitiesByCountryIdAsync(countryId);
            return _mapper.Map<IEnumerable<CityDTO>>(cities);
        }
    }
}