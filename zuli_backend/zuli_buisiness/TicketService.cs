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
            if (request.FlightId == null)
                throw new ZuliNotFoundException("Vuelo no encontrado");

            var flight = await _flightRepository.GetFlightById(request.FlightId.Value);
            if (flight == null)
                throw new ZuliNotFoundException("Vuelo no encontrado");

            FlightEntity? returnFlight = null;
            if (request.ReturnFlightId != null)
            {
                returnFlight = await _flightRepository.GetFlightById(request.ReturnFlightId.Value);
            }

            var totalPayment = CalculateTotalPayment(request, flight, returnFlight);
            var reservationCode = GenerateReservationCode();

            var buyerId = await CreateBuyer(request.Buyer);
            var passengerIds = await CreateAllPassengers(request.Passengers);

            var reservationId = await CreateReservation(reservationCode, request, totalPayment, buyerId);
            await LinkPassengersToReservation(passengerIds, reservationId);
            await CreateAllBoardingPasses(reservationCode, passengerIds, request);
            await RegisterAllBaggage(request.Passengers, passengerIds, reservationId);

            return new TicketPurchaseResponseDTO
            {
                ConfirmationCode = reservationCode,
                ReservationId = reservationId,
                TotalPayment = totalPayment,
                Message = "Compra realizada exitosamente"
            };
        }

        private async Task<int> CreateBuyer(BuyerTicketDTO buyerDto)
        {
            var buyer = _mapper.Map<BuyerEntity>(buyerDto);
            return await _buyerRepository.CreateBuyer(buyer);
        }

        private async Task CreateAllBoardingPasses(string reservationCode, List<int> passengerIds, TicketPurchaseRequestDTO request)
        {
            var flightIds = new List<Guid> { request.FlightId!.Value };
            if (request.ReturnFlightId != null)
                flightIds.Add(request.ReturnFlightId.Value);

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

        private static decimal CalculateTotalPayment(TicketPurchaseRequestDTO request, FlightEntity flight, FlightEntity? returnFlight = null)
        {
            var isFirstClass = request.FlightClass.Equals("Primera Clase", StringComparison.OrdinalIgnoreCase);

            static decimal FlightTotal(FlightEntity f, PassengerTicketDTO p, bool firstClass)
            {
                var classPrice = firstClass ? f.FirstClassPrice : f.TouristPrice;
                var checkedPrice = f.CheckedPrice ?? 0;
                var carryOnPrice = f.CarryOnPrice ?? 0;
                var multiplier = f.CheckedBagMultiplier > 0 ? f.CheckedBagMultiplier : 1m;
                return classPrice + (p.CheckedBaggage * checkedPrice * multiplier) + (p.CarryOn * carryOnPrice);
            }

            var outboundTotal = request.Passengers.Sum(p => FlightTotal(flight, p, isFirstClass));

            if (returnFlight != null)
            {
                outboundTotal += request.Passengers.Sum(p => FlightTotal(returnFlight, p, isFirstClass));
            }

            return outboundTotal;
        }

        private async Task<List<int>> CreateAllPassengers(List<PassengerTicketDTO> passengers)
        {
            var ids = new List<int>();
            foreach (var p in passengers)
            {
                var person = p.Adapt<PersonEntity>();
                var personId = await _personRepository.CreatePerson(person);

                var passport = new PassportEntity
                {
                    PassengerId = personId,
                    DueDate = DateTime.Parse(p.PassportDueDate),
                    PassportCountry = p.PassportCountry
                };
                await _personRepository.CreatePassport(passport);

                ids.Add(personId);
            }
            return ids;
        }

        private async Task LinkPassengersToReservation(List<int> passengerIds, int reservationId)
        {
            foreach (var pid in passengerIds)
            {
                await _reservationRepository.CreatePassengerReservation(new PassengerReservationEntity
                {
                    PassengerId = pid,
                    ReservationId = reservationId
                });
            }
        }

        private async Task RegisterAllBaggage(List<PassengerTicketDTO> passengers, List<int> passengerIds, int reservationId)
        {
            for (int i = 0; i < passengers.Count; i++)
            {
                var dto = passengers[i];
                var passengerId = passengerIds[i];

                foreach (var bag in dto.BaggageItems)
                {
                    var baggageEntity = bag.Adapt<BaggageEntity>();
                    baggageEntity.PassengerId = passengerId;
                    baggageEntity.ReservationId = reservationId;
                    await _baggageRepository.CreateBaggage(baggageEntity);
                }

                if (dto.CarryOn == 1)
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

        private async Task<int> CreateReservation(string code, TicketPurchaseRequestDTO request, decimal total, int buyerId)
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

        private static string GenerateReservationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
    }
}