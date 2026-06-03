using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class PassengerValidationService : IPassengerValidationService
    {
        private readonly IReservationRepository _reservationRepository;

        public PassengerValidationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public Task ValidateRequestPassengersAreUnique(List<PassengerTicketDTO> passengers)
        {
            var passengerKeys = new HashSet<string>();
            var errors = new Dictionary<string, List<string>>();

            for (var i = 0; i < passengers.Count; i++)
            {
                var passengerKey = StringHelper.BuildPassengerKey(passengers[i]);

                if (passengerKeys.Add(passengerKey))
                {
                    continue;
                }

                errors[$"passengers[{i}]"] = new List<string>
                {
                    "Este pasajero está repetido en la compra actual."
                };
            }

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }

            return Task.CompletedTask;
        }

        public async Task ValidatePassengersDoNotExistInFlights(List<PassengerTicketDTO> passengers, 
            List<Guid> flightIds)
        {
            var passengerChecks = passengers.Select((p, i) => new PassengerCheckInfo(
                i,
                p.FirstName,
                p.FirstLastName,
                p.SecondLastName,
                p.BirthDate,
                p.PassportCountry
            )).ToList();

            var results = await _reservationRepository.PassengersExistInFlights(flightIds, passengerChecks);

            var errors = new Dictionary<string, List<string>>();

            for (int i = 0; i < passengers.Count; i++)
            {
                if (results.GetValueOrDefault(i, false))
                {
                    errors[$"passengers[{i}]"] = new List<string>
                    {
                        "Este pasajero ya tiene un espacio registrado en uno de los vuelos seleccionados."
                    };
                }
            }

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }
    }
}
