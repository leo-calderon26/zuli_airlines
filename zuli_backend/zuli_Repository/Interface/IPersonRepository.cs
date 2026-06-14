using zuli_Data;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IPersonRepository
    {
        Task<PersonEntity?> GetPersonByEmail(string email);
        Task<List<PersonBulkResult>> CreatePersonBulk(
            List<PersonEntity> persons,
            List<PassportEntity> passports,
            BuyerEntity buyer,
            IUnitOfWork? uow = null);
    }
}
