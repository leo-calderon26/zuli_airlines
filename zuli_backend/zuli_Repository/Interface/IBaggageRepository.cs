using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IBaggageRepository
    {
        Task CreateBaggage(BaggageEntity baggage);
    }
}
