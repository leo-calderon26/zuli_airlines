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

        public async Task<List<int>> CreateAllPassengers(List<PassengerTicketDTO> passengers)
        {
            var passengerIds = new List<int>();

            foreach (var passenger in passengers)
            {
                var person = passenger.Adapt<PersonEntity>();
                var personId = await _personRepository.CreatePerson(person);

                var passport = new PassportEntity
                {
                    PassengerId = personId,
                    DueDate = DateTime.Parse(passenger.PassportDueDate),
                    PassportCountry = passenger.PassportCountry
                };

                await _personRepository.CreatePassport(passport);
                passengerIds.Add(personId);
            }

            return passengerIds;
        }
    }
}
