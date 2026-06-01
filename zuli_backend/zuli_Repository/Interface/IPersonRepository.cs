using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IPersonRepository
    {
        Task<int> CreatePerson(PersonEntity person);
        Task CreatePassport(PassportEntity passport);
        Task<PersonEntity?> GetPersonByEmail(string email);
    }
}
