using MapsterMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
namespace zuli_Business
{
    public class TicketPurchaseService : ITicketPurchaseService
    {
        private readonly IFlightResolverService _flightResolverService;
        private readonly IPassengerValidationService _passengerValidationService;
        private readonly IReservationCreationService _reservationCreationService;
        private readonly IPassengerCreationService _passengerCreationService;
        private readonly IBaggageRegistrationService _baggageRegistrationService;
        private readonly IReservationRepository _reservationRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IPurchaseConfirmationRepository _purchaseConfirmationRepository;
        private readonly IPurchaseConfirmationPdfService _purchaseConfirmationPdfService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public TicketPurchaseService(
            IFlightResolverService flightResolverService,
            IPassengerValidationService passengerValidationService,
            IReservationCreationService reservationCreationService,
            IPassengerCreationService passengerCreationService,
            IBaggageRegistrationService baggageRegistrationService,
            IReservationRepository reservationRepository,
            IBuyerRepository buyerRepository,
            IPurchaseConfirmationRepository purchaseConfirmationRepository,
            IPurchaseConfirmationPdfService purchaseConfirmationPdfService,
            IEmailService emailService,
            IMapper mapper)
        {
            _flightResolverService = flightResolverService;
            _passengerValidationService = passengerValidationService;
            _reservationCreationService = reservationCreationService;
            _passengerCreationService = passengerCreationService;
            _baggageRegistrationService = baggageRegistrationService;
            _reservationRepository = reservationRepository;
            _buyerRepository = buyerRepository;
            _purchaseConfirmationRepository = purchaseConfirmationRepository;
            _purchaseConfirmationPdfService = purchaseConfirmationPdfService;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<TicketPurchaseResponseDTO> Purchase(TicketPurchaseRequestDTO request)
        {
            await _passengerValidationService.ValidateRequestPassengersAreUnique(request.Passengers);

            var flightIds = await _flightResolverService.ResolveFlightIds(request);
            var flights = await _flightResolverService.GetFlights(flightIds);

            await _passengerValidationService.ValidatePassengersDoNotExistInFlights(
                request.Passengers,
                flightIds
            );

            await _baggageRegistrationService.ValidateBaggageCapacity(
                request.Passengers,
                flightIds
            );

            var totalPayment = CalculateTotalPayment(request, flights);
            var reservationCode = ReservationCodeGenerator.Generate();

            var buyerId = await CreateBuyer(request.Buyer);
            var passengerIds = await _passengerCreationService.CreateAllPassengers(request.Passengers);

            var reservationId = await _reservationCreationService.CreateReservation(
                reservationCode,
                request,
                totalPayment,
                buyerId
            );

            await LinkPassengersToReservation(passengerIds, reservationId);
            await CreateAllBoardingPasses(reservationCode, passengerIds, flightIds);

            await _baggageRegistrationService.RegisterAllBaggage(
                request.Passengers,
                passengerIds,
                reservationId
            );

            await SendPurchaseEmails(reservationCode);

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

        private async Task LinkPassengersToReservation(List<int> passengerIds, int reservationId)
        {
            foreach (var passengerId in passengerIds)
            {
                await _reservationRepository.CreatePassengerReservation(
                    new PassengerReservationEntity
                    {
                        PassengerId = passengerId,
                        ReservationId = reservationId
                    }
                );
            }
        }

        private async Task CreateAllBoardingPasses(string reservationCode, List<int> passengerIds,
            List<Guid> flightIds)
        {
            foreach (var passengerId in passengerIds)
            {
                foreach (var flightId in flightIds)
                {
                    await _reservationRepository.CreateBoardingPass(
                        new BoardingPassEntity
                        {
                            FlightId = flightId,
                            ReservationCode = reservationCode,
                            PassengerId = passengerId
                        }
                    );
                }
            }
        }

        internal static decimal CalculateTotalPayment(TicketPurchaseRequestDTO request, List<FlightEntity> flights)
        {
            var isFirstClass = request.FlightClass.Equals(
                "Primera Clase",
                StringComparison.OrdinalIgnoreCase
            );

            decimal total = 0;

            foreach (var flight in flights)
            {
                total += request.Passengers.Sum(passenger =>
                    CalculateFlightPassengerTotal(flight, passenger, isFirstClass)
                );
            }

            return total;
        }

        internal static decimal CalculateFlightPassengerTotal(FlightEntity flight, PassengerTicketDTO passenger,
            bool isFirstClass)
        {
            var classPrice = isFirstClass
                ? flight.FirstClassPrice
                : flight.TouristPrice;

            var checkedPrice = flight.CheckedPrice ?? 0;
            var carryOnPrice = flight.CarryOnPrice ?? 0;
            var multiplier = flight.CheckedBagMultiplier > 0 ? flight.CheckedBagMultiplier
                : 1m;

            decimal checkedBaggageTotal = 0;
            for (int i = 1; i <= passenger.CheckedBaggage; i++)
            {
                checkedBaggageTotal += checkedPrice * (decimal)Math.Pow((double)multiplier, i);
            }

            return classPrice + checkedBaggageTotal + (passenger.CarryOn * carryOnPrice);
        }

        private async Task SendPurchaseEmails(string reservationCode)
        {
            var confirmation = await _purchaseConfirmationRepository
                .GetPurchaseConfirmationAsync(reservationCode);

            if (confirmation == null)
            {
                throw new ZuliNotFoundException("No se encontró la confirmación de compra.");
            }

            var confirmationDto = _mapper.Map<PurchaseConfirmationPageDTO>(confirmation);

            var invoicePdf = _purchaseConfirmationPdfService.GenerateInvoicePdf(confirmationDto);
            var confirmationPdf = _purchaseConfirmationPdfService.GenerateConfirmationPdf(confirmationDto);

            await _emailService.SendInvoiceEmailAsync(
                confirmationDto.BuyerEmail,
                confirmationDto.BuyerName,
                confirmationDto.ReservationCode,
                invoicePdf
            );

            await _emailService.SendPurchaseConfirmationEmailAsync(
                confirmationDto.BuyerEmail,
                confirmationDto.BuyerName,
                confirmationDto.ReservationCode,
                confirmationPdf
            );
        }
    }
}
