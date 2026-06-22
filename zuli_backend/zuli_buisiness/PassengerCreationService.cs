using Mapster;
using zuli_Business.DTO;
using zuli_Business.Interface;
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
            BuyerTicketDTO buyer)
        {
            var persons = new List<PersonEntity>();
            var passports = new List<PassportEntity>();

            foreach (var passenger in passengers)
            {
                persons.Add(passenger.Adapt<PersonEntity>());
                passports.Add(passenger.Adapt<PassportEntity>());
            }
            var buyerEntity = buyer.Adapt<BuyerEntity>();
            var results = await _personRepository.CreatePersonBulk(persons, passports, buyerEntity);

            var buyerResult = results.First(r => r.IsBuyer);
            var passengerResults = results
                .Where(r => !r.IsBuyer)
                .OrderBy(r => r.RowIndex)
                .ToList();

            var passengerIds = passengerResults.Select(r => r.PersonId).ToList();
            var buyerId = buyerResult.BuyerId;

            return (passengerIds, buyerId);
        }
    }
}
