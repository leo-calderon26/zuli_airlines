using System.Transactions;
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

            var breakdown = CalculatePurchaseBreakdown(request, flights);
            var totalPayment = breakdown.GrandTotal;

            int reservationId = 0;
            string reservationCode = string.Empty;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _passengerCreationService
                    .CreateAllPassengers(request.Passengers, request.Buyer);
                var passengerIds = result.passengerIds;
                var buyerId = result.buyerId;

                var reservationResult = await _reservationCreationService.CreateReservation(
                    request,
                    totalPayment,
                    buyerId
                );
                reservationId = reservationResult.ReservationId;
                reservationCode = reservationResult.ReservationCode;

                var distinctPassengerIds = passengerIds.Distinct().ToList();

                var passengerReservations = distinctPassengerIds
                    .Select(pid => new PassengerReservationEntity
                    {
                        PassengerId = pid,
                        ReservationId = reservationId
                    })
                    .ToList();
                await _reservationRepository.CreatePassengerReservationsBulk(passengerReservations);

                var boardingPasses = distinctPassengerIds
                    .SelectMany(pid => flightIds.Select(fid => new BoardingPassEntity
                    {
                        FlightId = fid,
                        ReservationCode = reservationCode,
                        PassengerId = pid
                    }))
                    .ToList();
                await _reservationRepository.CreateBoardingPassesBulk(boardingPasses);

                await _baggageRegistrationService.RegisterAllBaggage(
                    request.Passengers,
                    passengerIds,
                    reservationId
                );

                scope.Complete();
            }

            await SendPurchaseEmails(reservationCode);

            return new TicketPurchaseResponseDTO
            {
                ConfirmationCode = reservationCode,
                ReservationId = reservationId,
                TotalPayment = totalPayment,
                Message = "Compra realizada exitosamente",
                Breakdown = breakdown
            };
        }

        public static PurchaseBreakdownDTO CalculatePurchaseBreakdown(TicketPurchaseRequestDTO request, List<FlightEntity> flights)
        {
            var isFirstClass = request.FlightClass.Equals(
                "Primera Clase",
                StringComparison.OrdinalIgnoreCase
            );

            var breakdown = new PurchaseBreakdownDTO();
            decimal grandTotal = 0;

            foreach (var flight in flights)
            {
                var flightBreakdown = new FlightBreakdownDTO
                {
                    FlightNumber = flight.Id.ToString(),
                    OriginAirportCode = "",
                    DestinationAirportCode = ""
                };

                decimal flightTotal = 0;

                foreach (var passenger in request.Passengers)
                {
                    var passengerBreakdown = CalculatePassengerBreakdown(flight, passenger, isFirstClass);
                    flightBreakdown.Passengers.Add(passengerBreakdown);
                    flightTotal += passengerBreakdown.PassengerTotal;
                }

                flightBreakdown.FlightTotal = flightTotal;
                breakdown.Flights.Add(flightBreakdown);
                grandTotal += flightTotal;
            }

            breakdown.GrandTotal = grandTotal;
            return breakdown;
        }

        public static PassengerBreakdownDTO CalculatePassengerBreakdown(FlightEntity flight, PassengerTicketDTO passenger, bool isFirstClass)
        {
            var classPrice = isFirstClass
                ? flight.FirstClassPrice
                : flight.TouristPrice;

            var checkedPrice = flight.CheckedPrice ?? 0;
            var carryOnPrice = flight.CarryOnPrice ?? 0;
            var multiplier = flight.CheckedBagMultiplier > 0 ? flight.CheckedBagMultiplier : 1m;

            var checkedBags = new List<BaggageBreakdownItemDTO>();
            decimal checkedBaggageTotal = 0;

            var bagPrice = checkedPrice;
            for (int i = 1; i <= passenger.CheckedBaggage; i++)
            {
                checkedBaggageTotal += bagPrice;
                checkedBags.Add(new BaggageBreakdownItemDTO
                {
                    BagNumber = i,
                    Type = "Maleta",
                    Price = bagPrice
                });
                bagPrice *= multiplier;
            }

            var carryOnTotal = passenger.CarryOn * carryOnPrice;
            var passengerTotal = classPrice + checkedBaggageTotal + carryOnTotal;

            return new PassengerBreakdownDTO
            {
                FullName = $"{passenger.FirstName} {passenger.FirstLastName} {passenger.SecondLastName}".Trim(),
                TicketPrice = classPrice,
                CheckedBags = checkedBags,
                CarryOnQuantity = passenger.CarryOn,
                CarryOnTotal = carryOnTotal,
                PassengerTotal = passengerTotal
            };
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
            confirmationDto.Breakdown = PurchaseConfirmationService.BuildPurchaseBreakdown(confirmationDto);

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