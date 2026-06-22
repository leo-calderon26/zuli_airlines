using zuli_Data.Entities.Filters;
namespace zuli_Repository.Interface
{
    public interface IFilterOptionsRepository
    {        
        Task<FilterOptionsEntity> GetFilterOptionsAsync();
    }
}

