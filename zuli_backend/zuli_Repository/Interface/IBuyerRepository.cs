using System.Threading.Tasks;
using zuli_Data;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IBuyerRepository
    {
        Task<int> CreateBuyer(BuyerEntity buyer, IUnitOfWork? uow = null);
    }
}
