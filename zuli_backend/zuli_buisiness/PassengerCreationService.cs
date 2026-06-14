using Mapster;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class PassengerCreationService : IPassengerCreationService
    {
        private readonly IPersonRepository _personRepository;

        public PassengerCreationService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }
        
        public async Task<(List<int> passengerIds, int buyerId)> CreateAllPassengers(
            List<PassengerTicketDTO> passengers,
            BuyerTicketDTO buyer,
            IUnitOfWork? uow = null)
        {
            var persons = new List<PersonEntity>();
            var passports = new List<PassportEntity>();

            foreach (var passenger in passengers)
            {
                persons.Add(passenger.Adapt<PersonEntity>());
                passports.Add(passenger.Adapt<PassportEntity>());
            }
            var buyerEntity = buyer.Adapt<BuyerEntity>();
            var results = await _personRepository.CreatePersonBulk(persons, passports, buyerEntity, uow);
            
            var buyerResult = results.First();
            var passengerResults = results.Skip(1).ToList();

            var passengerIds = passengerResults.Select(r => r.PersonId).ToList();
            var buyerId = buyerResult.BuyerId;

            return (passengerIds, buyerId);
        }
    }
}
