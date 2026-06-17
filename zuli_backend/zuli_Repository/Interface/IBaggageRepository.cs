using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IBaggageRepository
    {
        Task CreateBaggageBulk(List<BaggageEntity> baggages);
    }
}
