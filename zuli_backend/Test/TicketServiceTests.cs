using MapsterMapper;
using Moq;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Test
{
    [TestFixture]
    public class TicketServiceTests
    {
        private Mock<IPersonRepository> _personRepoMock;
        private Mock<IReservationRepository> _reservationRepoMock;
        private Mock<IBaggageRepository> _baggageRepoMock;
        private Mock<IFlightRepository> _flightRepoMock;
        private Mock<IBuyerRepository> _buyerRepoMock;
        private Mock<IMapper> _mapperMock;

        private TicketService _ticketService;

        private readonly Guid _flightId = Guid.NewGuid();
        private readonly Guid _returnFlightId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _personRepoMock = new Mock<IPersonRepository>();
            _reservationRepoMock = new Mock<IReservationRepository>();
            _baggageRepoMock = new Mock<IBaggageRepository>();
            _flightRepoMock = new Mock<IFlightRepository>();
            _buyerRepoMock = new Mock<IBuyerRepository>();
            _mapperMock = new Mock<IMapper>();

            _mapperMock
                .Setup(m => m.Map<BuyerEntity>(It.IsAny<BuyerTicketDTO>()))
                .Returns<BuyerTicketDTO>(dto => new BuyerEntity
                {
                    FirstName = dto.FirstName,
                    FirstLastName = dto.FirstLastName,
                    SecondLastName = dto.SecondLastName,
                    BirthDate = dto.BirthDate,
                    Email = dto.Email,
                    Phone = dto.Phone
                });

            _ticketService = new TicketService(
                _personRepoMock.Object,
                _reservationRepoMock.Object,
                _baggageRepoMock.Object,
                _flightRepoMock.Object,
                _buyerRepoMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public void Purchase_EmptyFlightRoutes_ThrowsZuliNotFoundException()
        {
            var request = BuildValidRequest(flightRoutes: new List<SummarizedFlightRoute>());

            Assert.That(async () => await _ticketService.Purchase(request), Throws.TypeOf<ZuliNotFoundException>());

            _flightRepoMock.Verify(r => r.GetFlightByRoute(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _flightRepoMock.Verify(r => r.GetFlightById(It.IsAny<Guid>()), Times.Never);
            _personRepoMock.Verify(r => r.CreatePerson(It.IsAny<PersonEntity>()), Times.Never);
        }

        [Test]
        public void Purchase_FlightNotFound_ThrowsZuliNotFoundException()
        {
            var request = BuildValidRequest();

            _flightRepoMock
                .Setup(r => r.GetFlightByRoute(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(_flightId);

            _flightRepoMock
                .Setup(r => r.GetFlightById(_flightId))
                .ReturnsAsync((FlightEntity?)null);

            Assert.That(async () => await _ticketService.Purchase(request), Throws.TypeOf<ZuliNotFoundException>());
        }

        [Test]
        public async Task Purchase_SuccessfulOneWay_CreatesReservationAndReturnsConfirmation()
        {
            var flight = BuildFlight(touristPrice: 200m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest();

            var result = await _ticketService.Purchase(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReservationId, Is.EqualTo(100));
            Assert.That(result.TotalPayment, Is.EqualTo(200m));
            Assert.That(result.ConfirmationCode, Has.Length.EqualTo(8));
            Assert.That(result.Message, Is.EqualTo("Compra realizada exitosamente"));

            _buyerRepoMock.Verify(r => r.CreateBuyer(It.IsAny<BuyerEntity>()), Times.Once);
            _personRepoMock.Verify(r => r.CreatePerson(It.IsAny<PersonEntity>()), Times.Once);
            _personRepoMock.Verify(r => r.CreatePassport(It.IsAny<PassportEntity>()), Times.Once);
            _reservationRepoMock.Verify(r => r.CreateReservation(It.IsAny<ReservationEntity>()), Times.Once);
            _reservationRepoMock.Verify(r => r.CreatePassengerReservation(It.IsAny<PassengerReservationEntity>()), Times.Once);
            _reservationRepoMock.Verify(r => r.CreateBoardingPass(It.IsAny<BoardingPassEntity>()), Times.Once);
        }

        [Test]
        public async Task Purchase_SuccessfulRoundTrip_CreatesBoardingPassesForBothFlights()
        {
            var flight = BuildFlight(touristPrice: 200m);
            var returnFlight = BuildFlight(touristPrice: 150m, id: _returnFlightId);
            SetupFlightMocks(flight, returnFlight);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest(returnFlightId: _returnFlightId);

            var result = await _ticketService.Purchase(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalPayment, Is.EqualTo(350m));

            _reservationRepoMock.Verify(r => r.CreateBoardingPass(It.IsAny<BoardingPassEntity>()), Times.Exactly(2));
            _flightRepoMock.Verify(r => r.GetFlightById(_returnFlightId), Times.Once);
        }

        [Test]
        public async Task Purchase_MultiplePassengers_CreatesPersonAndPassportForEach()
        {
            var flight = BuildFlight(touristPrice: 100m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .SetupSequence(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1)
                .ReturnsAsync(2);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest(passengerCount: 2);

            var result = await _ticketService.Purchase(request);

            Assert.That(result.TotalPayment, Is.EqualTo(200m));

            _buyerRepoMock.Verify(r => r.CreateBuyer(It.IsAny<BuyerEntity>()), Times.Once);
            _personRepoMock.Verify(r => r.CreatePerson(It.IsAny<PersonEntity>()), Times.Exactly(2));
            _personRepoMock.Verify(r => r.CreatePassport(It.IsAny<PassportEntity>()), Times.Exactly(2));
            _reservationRepoMock.Verify(r => r.CreatePassengerReservation(It.IsAny<PassengerReservationEntity>()), Times.Exactly(2));
            _reservationRepoMock.Verify(r => r.CreateBoardingPass(It.IsAny<BoardingPassEntity>()), Times.Exactly(2));
        }

        [Test]
        public async Task Purchase_WithCheckedBaggage_RegistersBaggageItems()
        {
            var flight = BuildFlight(touristPrice: 100m, checkedPrice: 30m, checkedBagMultiplier: 1.5m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest(checkedBaggage: 2);

            var result = await _ticketService.Purchase(request);

            var expectedBaggageTotal = 2 * 30m * 1.5m;
            Assert.That(result.TotalPayment, Is.EqualTo(100m + expectedBaggageTotal));

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.Type == "Maleta")), Times.Exactly(2));
        }

        [Test]
        public async Task Purchase_WithCarryOn_RegistersCarryOnBaggage()
        {
            var flight = BuildFlight(touristPrice: 100m, carryOnPrice: 15m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest(carryOn: 1);

            var result = await _ticketService.Purchase(request);

            Assert.That(result.TotalPayment, Is.EqualTo(100m + 15m));

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.Type == "Mano")), Times.Once);
        }

        [Test]
        public async Task Purchase_FirstClass_UsesFirstClassPrice()
        {
            var flight = BuildFlight(touristPrice: 100m, firstClassPrice: 300m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest(flightClass: "Primera Clase");

            var result = await _ticketService.Purchase(request);

            Assert.That(result.TotalPayment, Is.EqualTo(300m));
        }

        [Test]
        public async Task Purchase_BuyerDataIsSavedCorrectly()
        {
            var flight = BuildFlight(touristPrice: 100m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(1);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest();

            await _ticketService.Purchase(request);

            _buyerRepoMock.Verify(r => r.CreateBuyer(It.Is<BuyerEntity>(b =>
                b.Phone == "+506 8888 8888"
            )), Times.Once);
        }

        [Test]
        public async Task Purchase_ReservationUsesBuyerId()
        {
            var flight = BuildFlight(touristPrice: 100m);
            SetupFlightMocks(flight, null);

            _personRepoMock
                .Setup(r => r.CreatePerson(It.IsAny<PersonEntity>()))
                .ReturnsAsync(1);

            _buyerRepoMock
                .Setup(r => r.CreateBuyer(It.IsAny<BuyerEntity>()))
                .ReturnsAsync(50);

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var request = BuildValidRequest();

            await _ticketService.Purchase(request);

            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                res.BuyerId == 50
            )), Times.Once);
        }

        private TicketPurchaseRequestDTO BuildValidRequest(
            List<SummarizedFlightRoute>? flightRoutes = null,
            Guid? returnFlightId = null,
            string flightClass = "Turista",
            int passengerCount = 1,
            int checkedBaggage = 0,
            int carryOn = 0)
        {
            var passengers = new List<PassengerTicketDTO>();
            for (int i = 0; i < passengerCount; i++)
            {
                var baggageItems = new List<BaggageItemDTO>();
                for (int j = 0; j < checkedBaggage; j++)
                {
                    baggageItems.Add(new BaggageItemDTO
                    {
                        Weight = 23.0m,
                        Size = "Mediano",
                        Type = "Maleta"
                    });
                }

                passengers.Add(new PassengerTicketDTO
                {
                    FirstName = $"Juan{i + 1}",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    Gender = "Male",
                    PassportCountry = "Costa Rica",
                    PassportDueDate = "2028-12-31",
                    CheckedBaggage = checkedBaggage,
                    BaggageItems = baggageItems,
                    CarryOn = carryOn
                });
            }

            return new TicketPurchaseRequestDTO
            {
                FlightRoutes = flightRoutes ?? new List<SummarizedFlightRoute>
                {
                    new SummarizedFlightRoute { FlightRouteId = 1, DepartureDate = "2026-07-15" }
                },
                ReturnFlightId = returnFlightId,
                FlightClass = flightClass,
                Passengers = passengers,
                Buyer = new BuyerTicketDTO
                {
                    FirstName = "Comprador",
                    FirstLastName = "Test",
                    SecondLastName = "Apellido",
                    BirthDate = "1990-01-01",
                    Email = "test@example.com",
                    Phone = "+506 8888 8888"
                },
                PaymentMethod = "card",
                ReservationOrigin = "Web"
            };
        }

        private FlightEntity BuildFlight(
            Guid? id = null,
            decimal touristPrice = 200m,
            decimal firstClassPrice = 400m,
            decimal? checkedPrice = 30m,
            decimal? carryOnPrice = 15m,
            decimal checkedBagMultiplier = 1.0m)
        {
            return new FlightEntity
            {
                Id = id ?? _flightId,
                TouristPrice = touristPrice,
                FirstClassPrice = firstClassPrice,
                CheckedPrice = checkedPrice,
                CarryOnPrice = carryOnPrice,
                CheckedBagMultiplier = checkedBagMultiplier,
                AvailableSeats = 10,
                FlightDate = new DateTime(2026, 7, 15),
                Status = "Activo",
                Duration = 120,
                FlightRouteId = 1
            };
        }

        private void SetupFlightMocks(FlightEntity? flight, FlightEntity? returnFlight)
        {
            _flightRepoMock
                .Setup(r => r.GetFlightByRoute(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(_flightId);

            _flightRepoMock
                .Setup(r => r.GetFlightById(_flightId))
                .ReturnsAsync(flight);

            if (returnFlight != null)
            {
                _flightRepoMock
                    .Setup(r => r.GetFlightById(_returnFlightId))
                    .ReturnsAsync(returnFlight);
            }
        }
    }
}