using Mapster;
using MapsterMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class TicketService : ITicketService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IBaggageRepository _baggageRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IMapper _mapper;

        public TicketService(
            IPersonRepository personRepository,
            IReservationRepository reservationRepository,
            IBaggageRepository baggageRepository,
            IFlightRepository flightRepository,
            IBuyerRepository buyerRepository,
            IMapper mapper)
        {
            _personRepository = personRepository;
            _reservationRepository = reservationRepository;
            _baggageRepository = baggageRepository;
            _flightRepository = flightRepository;
            _buyerRepository = buyerRepository;
            _mapper = mapper;
        }

        public async Task<TicketPurchaseResponseDTO> Purchase(TicketPurchaseRequestDTO request)
        {
            var flightIds = await ResolveFlightIds(request);
            var flights = await GetFlights(flightIds);

            var totalPayment = CalculateTotalPayment(request, flights);
            var reservationCode = GenerateReservationCode();

            var buyerId = await CreateBuyer(request.Buyer);
            var passengerIds = await CreateAllPassengers(request.Passengers);

            var reservationId = await CreateReservation(
                reservationCode,
                request,
                totalPayment,
                buyerId
            );

            await LinkPassengersToReservation(passengerIds, reservationId);
            await CreateAllBoardingPasses(reservationCode, passengerIds, flightIds);
            await RegisterAllBaggage(request.Passengers, passengerIds, reservationId);

            return new TicketPurchaseResponseDTO
            {
                ConfirmationCode = reservationCode,
                ReservationId = reservationId,
                TotalPayment = totalPayment,
                Message = "Compra realizada exitosamente"
            };
        }

        private async Task<List<Guid>> ResolveFlightIds(TicketPurchaseRequestDTO request)
        {
            var flightIds = new List<Guid>();

            foreach (var flightRoute in request.FlightRoutes)
            {
                var flightId = await ResolveFlightId(flightRoute);
                flightIds.Add(flightId);
            }

            if (request.FlightId.HasValue)
            {
                flightIds.Add(request.FlightId.Value);
            }

            if (request.ReturnFlightId.HasValue)
            {
                flightIds.Add(request.ReturnFlightId.Value);
            }

            var distinctFlightIds = flightIds
                .Where(flightId => flightId != Guid.Empty)
                .Distinct()
                .ToList();

            if (distinctFlightIds.Count == 0)
            {
                throw new ZuliNotFoundException("Vuelo no encontrado");
            }

            request.FlightIdList = distinctFlightIds;

            return distinctFlightIds;
        }

        private async Task<Guid> ResolveFlightId(SummarizedFlightRoute flightRoute)
        {
            var existingFlightId = await _flightRepository.GetFlightByRoute(
                flightRoute.FlightRouteId,
                flightRoute.DepartureDate
            );

            if (existingFlightId != Guid.Empty)
            {
                return existingFlightId;
            }

            return await _flightRepository.CreateFlight(
                flightRoute.FlightRouteId,
                flightRoute.DepartureDate
            );
        }

        private async Task<List<FlightEntity>> GetFlights(List<Guid> flightIds)
        {
            var flights = new List<FlightEntity>();

            foreach (var flightId in flightIds)
            {
                var flight = await _flightRepository.GetFlightById(flightId);

                if (flight == null)
                {
                    throw new ZuliNotFoundException("Vuelo no encontrado");
                }

                flights.Add(flight);
            }

            return flights;
        }

        private async Task<int> CreateBuyer(BuyerTicketDTO buyerDto)
        {
            var buyer = _mapper.Map<BuyerEntity>(buyerDto);
            return await _buyerRepository.CreateBuyer(buyer);
        }

        private async Task<List<int>> CreateAllPassengers(List<PassengerTicketDTO> passengers)
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

        private async Task LinkPassengersToReservation(
            List<int> passengerIds,
            int reservationId)
        {
            foreach (var passengerId in passengerIds)
            {
                await _reservationRepository.CreatePassengerReservation(new PassengerReservationEntity
                {
                    PassengerId = passengerId,
                    ReservationId = reservationId
                });
            }
        }

        private async Task CreateAllBoardingPasses(
            string reservationCode,
            List<int> passengerIds,
            List<Guid> flightIds)
        {
            foreach (var passengerId in passengerIds)
            {
                foreach (var flightId in flightIds)
                {
                    await _reservationRepository.CreateBoardingPass(new BoardingPassEntity
                    {
                        FlightId = flightId,
                        ReservationCode = reservationCode,
                        PassengerId = passengerId
                    });
                }
            }
        }

        private async Task RegisterAllBaggage(
            List<PassengerTicketDTO> passengers,
            List<int> passengerIds,
            int reservationId)
        {
            for (int i = 0; i < passengers.Count; i++)
            {
                var passenger = passengers[i];
                var passengerId = passengerIds[i];

                foreach (var bag in passenger.BaggageItems)
                {
                    var baggageEntity = bag.Adapt<BaggageEntity>();
                    baggageEntity.PassengerId = passengerId;
                    baggageEntity.ReservationId = reservationId;

                    await _baggageRepository.CreateBaggage(baggageEntity);
                }

                if (passenger.CarryOn == 1)
                {
                    await _baggageRepository.CreateBaggage(new BaggageEntity
                    {
                        PassengerId = passengerId,
                        ReservationId = reservationId,
                        Weight = 7.0m,
                        Size = "Pequeño",
                        Type = "Mano"
                    });
                }
            }
        }

        private async Task<int> CreateReservation(
            string code,
            TicketPurchaseRequestDTO request,
            decimal total,
            int buyerId)
        {
            var reservation = new ReservationEntity
            {
                ReservationCode = code,
                ReservationOrigin = request.ReservationOrigin,
                TotalPayment = total,
                PurchaseDate = DateTime.Now.Date,
                BuyerId = buyerId,
                FlightClass = request.FlightClass,
                PaymentMethod = request.PaymentMethod
            };

            return await _reservationRepository.CreateReservation(reservation);
        }

        private static decimal CalculateTotalPayment(
            TicketPurchaseRequestDTO request,
            List<FlightEntity> flights)
        {
            var isFirstClass = request.FlightClass.Equals(
                "Primera Clase",
                StringComparison.OrdinalIgnoreCase
            );

            decimal total = 0;

            foreach (var flight in flights)
            {
                total += request.Passengers.Sum(passenger =>
                    CalculateFlightPassengerTotal(
                        flight,
                        passenger,
                        isFirstClass
                    )
                );
            }

            return total;
        }

        private static decimal CalculateFlightPassengerTotal(
            FlightEntity flight,
            PassengerTicketDTO passenger,
            bool isFirstClass)
        {
            var classPrice = isFirstClass
                ? flight.FirstClassPrice
                : flight.TouristPrice;

            var checkedPrice = flight.CheckedPrice ?? 0;
            var carryOnPrice = flight.CarryOnPrice ?? 0;
            var multiplier = flight.CheckedBagMultiplier > 0
                ? flight.CheckedBagMultiplier
                : 1m;

            return classPrice
                + (passenger.CheckedBaggage * checkedPrice * multiplier)
                + (passenger.CarryOn * carryOnPrice);
        }

        private static string GenerateReservationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            return new string(
                Enumerable.Range(0, 8)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray()
            );
        }
    }
}