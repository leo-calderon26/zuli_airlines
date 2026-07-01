using zuli_Business.DTO;
using zuli_Business.DTO.ReservationSearch;
using zuli_Business.Interface;
using zuli_Data.DTO;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class BaggageRegistrationService : IBaggageRegistrationService
    {
        private const decimal DEFAULT_CHECKED_BAGGAGE_WEIGHT = 23.0m;
        private const decimal DEFAULT_CARRY_ON_WEIGHT = 7.0m;
        private const int MAX_CHECKED_BAGGAGER_PASSENGER = 5;
        private const int MAX_CARRY_ON_PER_PASSENGER = 1;

        private readonly IBaggageRepository _baggageRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IEmailJobQueue? _emailJobQueue;

        public BaggageRegistrationService(
            IBaggageRepository baggageRepository,
            IFlightRepository flightRepository,
            IEmailJobQueue? emailJobQueue = null)
        {
            _baggageRepository = baggageRepository;
            _flightRepository = flightRepository;
            _emailJobQueue = emailJobQueue;
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
                requestedBaggageWeight / DEFAULT_CHECKED_BAGGAGE_WEIGHT
            );

            foreach (var flightId in flightIds)
            {
                var remainingCapacity = await _flightRepository.GetRemainingBaggageCapacity(flightId);

                if (requestedBaggageWeight <= remainingCapacity)
                {
                    continue;
                }

                var maxAvailableBags = Math.Floor(
                    remainingCapacity / DEFAULT_CHECKED_BAGGAGE_WEIGHT
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
            var baggages = BuildBaggageList(passengers, passengerIds, reservationId);
            if (baggages.Count > 0)
            {
                await _baggageRepository.CreateBaggageBulk(baggages);
            }
        }

        private List<BaggageEntity> BuildBaggageList(List<PassengerTicketDTO> passengers, List<int> passengerIds, int reservationId)
        {
            ValidatePassengerBaggageLimits(passengers);
            var baggages = new List<BaggageEntity>();

            for (int i = 0; i < passengers.Count ; i++)
            {
                for (int b = 0; b < passengers[i].CheckedBaggage; b++)
                {
                    var bag = passengers[i].BaggageItems.ElementAtOrDefault(b);
                    baggages.Add(new BaggageEntity
                    {
                        PassengerId = passengerIds[i],
                        ReservationId = reservationId,
                        Weight = bag?.Weight > 0 ? bag.Weight : DEFAULT_CHECKED_BAGGAGE_WEIGHT,
                        Size = string.IsNullOrWhiteSpace(bag?.Size) ? "Mediano" : bag.Size,
                        Type = "Maleta"
                    });
                }
                for (int c = 0; c < passengers[i].CarryOn; c++)
                {
                    baggages.Add(new BaggageEntity
                    {
                        PassengerId = passengerIds[i],
                        ReservationId = reservationId,
                        Weight = DEFAULT_CARRY_ON_WEIGHT,
                        Size = "Pequeño",
                        Type = "Mano"
                    });
                }
            }
            return baggages;
        }

        private static void ValidatePassengerBaggageLimits(
            List<PassengerTicketDTO> passengers)
        {
            var errors = new Dictionary<string, List<string>>();

            for (int i = 0; i < passengers.Count; i++)
            {
                var passenger = passengers[i];

                if (passenger.CheckedBaggage < 0 || passenger.CheckedBaggage > MAX_CHECKED_BAGGAGER_PASSENGER)
                {
                    errors[$"passengers[{i}].checkedBaggage"] = new List<string>
                    {
                        $"Cada pasajero puede llevar máximo {MAX_CHECKED_BAGGAGER_PASSENGER} maletas documentadas."
                    };
                }

                if (passenger.CarryOn < 0 || passenger.CarryOn > MAX_CARRY_ON_PER_PASSENGER)
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
                passenger.CheckedBaggage * DEFAULT_CHECKED_BAGGAGE_WEIGHT
            );
        }

        public async Task<AdditionalBaggagePurchaseResultDTO> AddAdditionalBaggageTransactional(
            string reservationCode,
            List<AdditionalBaggagePassengerDTO> passengers)
        {
            reservationCode = reservationCode.Trim().ToUpperInvariant();
            var baggages = new List<BaggageEntity>();
            var additionalCheckedBaggage = 0;
            var additionalCarryOn = 0;

            foreach (var passenger in passengers)
            {
                additionalCheckedBaggage += passenger.AdditionalCheckedBaggage;
                additionalCarryOn += passenger.AdditionalCarryOn;

                for (int baggage = 0; baggage < passenger.AdditionalCheckedBaggage; baggage++)
                {
                    baggages.Add(new BaggageEntity
                    {
                        PassengerId = passenger.PassengerId,
                        Weight = DEFAULT_CHECKED_BAGGAGE_WEIGHT,
                        Size = "Mediano",
                        Type = "Maleta"
                    });
                }

                for (int baggageSmall = 0; baggageSmall < passenger.AdditionalCarryOn; baggageSmall++)
                {
                    baggages.Add(new BaggageEntity
                    {
                        PassengerId = passenger.PassengerId,
                        Weight = DEFAULT_CARRY_ON_WEIGHT,
                        Size = "Pequeño",
                        Type = "Mano"
                    });
                }
            }

            if (baggages.Count == 0)
            {
                throw new ZuliValidationException("baggage", "No se proporcionó equipaje adicional para agregar.");
            }

            var result = await _baggageRepository.AddAdditionalBaggageTransactional(reservationCode, baggages);

            if (_emailJobQueue != null)
            {
                await _emailJobQueue.QueueAsync(new EmailJobDTO
                {
                    Type = EmailJobType.AdditionalBaggagePurchase,
                    ReservationCode = reservationCode,
                    AdditionalCheckedBaggage = additionalCheckedBaggage,
                    AdditionalCarryOn = additionalCarryOn,
                    AdditionalBaggageTotal = result.AdditionalBaggageTotal,
                    ReservationTotal = result.ReservationTotal
                });
            }

            return result;
        }
    }
}
