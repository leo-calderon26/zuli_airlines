
using zuli_Business.DTO.Filters;

namespace zuli_Business.Interface
{
    public interface IFilterOptionsService
    {
        Task<FilterOptionsDTO> GetFilterOptionsAsync();
    }
}
