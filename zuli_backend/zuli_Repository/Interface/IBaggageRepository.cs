using zuli_Data.DTO;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IBaggageRepository
    {
        Task CreateBaggageBulk(List<BaggageEntity> baggages);
        Task<AdditionalBaggagePurchaseResultDTO> AddAdditionalBaggageTransactional(string reservationCode, List<BaggageEntity> baggages);
    }
}
