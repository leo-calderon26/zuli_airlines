using MapsterMapper;
using zuli_Business.Interface;
using zuli_Business.DTO.Filters;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FilterOptionsService : IFilterOptionsService
    {
        private readonly IFilterOptionsRepository _filterOptionsRepository;
        private readonly IMapper _mapper;

        public FilterOptionsService(IFilterOptionsRepository filterOptionsRepository, IMapper mapper)
        {
            _filterOptionsRepository = filterOptionsRepository;
            _mapper = mapper;
        }

        public async Task<FilterOptionsDTO> GetFilterOptionsAsync()
        {
            var entity = await _filterOptionsRepository.GetFilterOptionsAsync();
            return _mapper.Map<FilterOptionsDTO>(entity);
        }
    }
}
