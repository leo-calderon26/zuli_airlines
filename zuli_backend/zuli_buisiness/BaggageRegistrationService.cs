using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class BaggageRegistrationService : IBaggageRegistrationService
    {
        private const decimal DefaultCheckedBaggageWeight = 23.0m;
        private const decimal DefaultCarryOnWeight = 7.0m;
        private const int MaxCheckedBaggagePerPassenger = 5;
        private const int MaxCarryOnPerPassenger = 1;

        private readonly IBaggageRepository _baggageRepository;
        private readonly IFlightRepository _flightRepository;

        public BaggageRegistrationService(
            IBaggageRepository baggageRepository,
            IFlightRepository flightRepository)
        {
            _baggageRepository = baggageRepository;
            _flightRepository = flightRepository;
        }

        public async Task ValidateBaggageCapacity(
            List<PassengerTicketDTO> passengers,
            List<Guid> flightIds)
        {
            var requestedBaggageWeight = CalculateRequestedCheckedBaggageWeight(passengers);

            if (requestedBaggageWeight <= 0)
            {
                return;
            }

            var requestedBaggageCount = Math.Ceiling(
                requestedBaggageWeight / DefaultCheckedBaggageWeight
            );

            foreach (var flightId in flightIds)
            {
                var remainingCapacity = await _flightRepository.GetRemainingBaggageCapacity(flightId);

                if (requestedBaggageWeight <= remainingCapacity)
                {
                    continue;
                }

                var maxAvailableBags = Math.Floor(
                    remainingCapacity / DefaultCheckedBaggageWeight
                );

                throw new ZuliValidationException(
                    "baggage",
                    $"Capacidad de equipaje insuficiente. Solo se permite {maxAvailableBags:0} maleta(s) adicional(es) en este vuelo."
                );
            }
        }

        public async Task RegisterAllBaggage(
            List<PassengerTicketDTO> passengers,
            List<int> passengerIds,
            int reservationId)
        {
            ValidatePassengerBaggageLimits(passengers);

            for (int i = 0; i < passengers.Count; i++)
            {
                await RegisterCheckedBaggage(
                    passengers[i],
                    passengerIds[i],
                    reservationId
                );

                await RegisterCarryOnBaggage(
                    passengers[i],
                    passengerIds[i],
                    reservationId
                );
            }
        }

        private static void ValidatePassengerBaggageLimits(
            List<PassengerTicketDTO> passengers)
        {
            var errors = new Dictionary<string, List<string>>();

            for (int i = 0; i < passengers.Count; i++)
            {
                var passenger = passengers[i];

                if (passenger.CheckedBaggage < 0 || passenger.CheckedBaggage > MaxCheckedBaggagePerPassenger)
                {
                    errors[$"passengers[{i}].checkedBaggage"] = new List<string>
                    {
                        $"Cada pasajero puede llevar máximo {MaxCheckedBaggagePerPassenger} maletas documentadas."
                    };
                }

                if (passenger.CarryOn < 0 || passenger.CarryOn > MaxCarryOnPerPassenger)
                {
                    errors[$"passengers[{i}].carryOn"] = new List<string>
                    {
                        "Cada pasajero puede llevar máximo un equipaje de mano."
                    };
                }
            }

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }

        private static decimal CalculateRequestedCheckedBaggageWeight(
            List<PassengerTicketDTO> passengers)
        {
            return passengers.Sum(passenger =>
                passenger.CheckedBaggage * DefaultCheckedBaggageWeight
            );
        }

        private async Task RegisterCheckedBaggage(
            PassengerTicketDTO passenger,
            int passengerId,
            int reservationId)
        {
            for (int i = 0; i < passenger.CheckedBaggage; i++)
            {
                var bag = passenger.BaggageItems.ElementAtOrDefault(i);

                await _baggageRepository.CreateBaggage(
                    new BaggageEntity
                    {
                        PassengerId = passengerId,
                        ReservationId = reservationId,
                        Weight = bag?.Weight > 0 ? bag.Weight : DefaultCheckedBaggageWeight,
                        Size = string.IsNullOrWhiteSpace(bag?.Size) ? "Mediano" : bag.Size,
                        Type = "Maleta"
                    }
                );
            }
        }

        private async Task RegisterCarryOnBaggage(
            PassengerTicketDTO passenger,
            int passengerId,
            int reservationId)
        {
            for (int i = 0; i < passenger.CarryOn; i++)
            {
                await _baggageRepository.CreateBaggage(
                    new BaggageEntity
                    {
                        PassengerId = passengerId,
                        ReservationId = reservationId,
                        Weight = DefaultCarryOnWeight,
                        Size = "Pequeño",
                        Type = "Mano"
                    }
                );
            }
        }
    }
}