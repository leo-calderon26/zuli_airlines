using Moq;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Test
{
    [TestFixture]
    public class TicketPurchaseServicesTests
    {
        private Mock<IReservationRepository> _reservationRepoMock;
        private Mock<IBaggageRepository> _baggageRepoMock;
        private Mock<IFlightRepository> _flightRepoMock;

        private PassengerValidationService _passengerValidationService;
        private BaggageRegistrationService _baggageRegistrationService;
        private ReservationCreationService _reservationCreationService;
        private FlightResolverService _flightResolverService;

        [SetUp]
        public void SetUp()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _baggageRepoMock = new Mock<IBaggageRepository>();
            _flightRepoMock = new Mock<IFlightRepository>();

            _reservationRepoMock
                .Setup(r => r.GetAllReservationCodes())
                .ReturnsAsync(new HashSet<string>());

            _passengerValidationService = new PassengerValidationService(_reservationRepoMock.Object);
            _baggageRegistrationService = new BaggageRegistrationService(_baggageRepoMock.Object, _flightRepoMock.Object);
            _reservationCreationService = new ReservationCreationService(_reservationRepoMock.Object);
            _flightResolverService = new FlightResolverService(_flightRepoMock.Object);
        }

        [Test]
        public void ResolveFlightIds_WithInsufficientSeats_ThrowsZuliBadRequestException()
        {
            var request = BuildTicketPurchaseRequest(passengerCount: 2);

            _flightRepoMock
                .Setup(r => r.CheckAvailability(1, It.IsAny<DateTime>(), 2))
                .ReturnsAsync(0);

            Assert.That(
                async () => await _flightResolverService.ResolveFlightIds(request),
                Throws.TypeOf<ZuliBadRequestException>()
                    .With.Property(nameof(ZuliBadRequestException.StatusCode)).EqualTo(400)
            );
        }

        [Test]
        public async Task ResolveFlightIds_WithAvailableSeats_ReturnsFlightIds()
        {
            var request = BuildTicketPurchaseRequest(passengerCount: 2);
            var flightId = Guid.NewGuid();

            _flightRepoMock
                .Setup(r => r.CheckAvailability(1, It.IsAny<DateTime>(), 2))
                .ReturnsAsync(1);
            _flightRepoMock
                .Setup(r => r.GetFlightByRoute(1, "2026-07-15"))
                .ReturnsAsync(flightId);

            var result = await _flightResolverService.ResolveFlightIds(request);

            Assert.That(result, Is.EquivalentTo(new[] { flightId }));
        }

        [Test]
        public void ValidateRequestPassengersAreUnique_WithUniquePassengers_DoesNotThrow()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica"
                },
                new PassengerTicketDTO
                {
                    FirstName = "Maria",
                    FirstLastName = "Lopez",
                    SecondLastName = "Sanchez",
                    BirthDate = "1985-06-20",
                    PassportCountry = "Mexico"
                }
            };

            Assert.That(async () => await _passengerValidationService.ValidateRequestPassengersAreUnique(passengers), Throws.Nothing);
        }

        [Test]
        public void ValidateRequestPassengersAreUnique_WithDuplicatePassengers_ThrowsZuliValidationException()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica"
                },
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica"
                }
            };

            Assert.That(async () => await _passengerValidationService.ValidateRequestPassengersAreUnique(passengers),
                Throws.TypeOf<ZuliValidationException>());
        }

        [Test]
        public void ValidateRequestPassengersAreUnique_WithSameNameDifferentBirthDate_DoesNotThrow()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica"
                },
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1992-05-20",
                    PassportCountry = "Costa Rica"
                }
            };

            Assert.That(async () => await _passengerValidationService.ValidateRequestPassengersAreUnique(passengers), Throws.Nothing);
        }

        [Test]
        public void ValidateRequestPassengersAreUnique_WithNormalizedNames_DetectsDuplicates()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "  Juan  ",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "costa rica"
                },
                new PassengerTicketDTO
                {
                    FirstName = "juan",
                    FirstLastName = "perez",
                    SecondLastName = "garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica"
                }
            };

            Assert.That(async () => await _passengerValidationService.ValidateRequestPassengersAreUnique(passengers),
                Throws.TypeOf<ZuliValidationException>());
        }

        [Test]
        public async Task RegisterAllBaggage_WithCheckedBaggage_CreatesCorrectBaggageItems()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CheckedBaggage = 2,
                    BaggageItems = new List<BaggageItemDTO>
                    {
                        new BaggageItemDTO { Weight = 25m, Size = "Grande", Type = "Maleta" },
                        new BaggageItemDTO { Weight = 20m, Size = "Mediano", Type = "Maleta" }
                    }
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggageBulk(It.Is<List<BaggageEntity>>(list =>
                list.Count == 2 &&
                list.Any(b => b.PassengerId == 1 && b.ReservationId == reservationId && b.Type == "Maleta" && b.Weight == 25m && b.Size == "Grande") &&
                list.Any(b => b.PassengerId == 1 && b.ReservationId == reservationId && b.Type == "Maleta" && b.Weight == 20m && b.Size == "Mediano")
            )), Times.Once);
        }

        [Test]
        public async Task RegisterAllBaggage_WithNoCheckedBaggage_DoesNotCreateCheckedBaggage()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CheckedBaggage = 0
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggageBulk(It.IsAny<List<BaggageEntity>>()), Times.Never);
        }

        [Test]
        public async Task RegisterAllBaggage_WithCarryOn_CreatesCarryOnBaggage()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CarryOn = 1
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggageBulk(It.Is<List<BaggageEntity>>(list =>
                list.Count == 1 &&
                list[0].PassengerId == 1 &&
                list[0].ReservationId == reservationId &&
                list[0].Type == "Mano" &&
                list[0].Weight == 7m &&
                list[0].Size == "Pequeño"
            )), Times.Once);
        }

        [Test]
        public async Task RegisterAllBaggage_WithMultiplePassengers_RegistersForEachPassenger()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CheckedBaggage = 1,
                    CarryOn = 1
                },
                new PassengerTicketDTO
                {
                    FirstName = "Maria",
                    FirstLastName = "Lopez",
                    SecondLastName = "Sanchez",
                    BirthDate = "1985-06-20",
                    PassportCountry = "Mexico",
                    CheckedBaggage = 2,
                    CarryOn = 0
                }
            };
            var passengerIds = new List<int> { 1, 2 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggageBulk(It.Is<List<BaggageEntity>>(list =>
                list.Count == 4 &&
                list.Count(b => b.PassengerId == 1) == 2 &&
                list.Count(b => b.PassengerId == 2) == 2
            )), Times.Once);
        }

        [Test]
        public async Task RegisterAllBaggage_ClampsCheckedBaggageToMax5()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CheckedBaggage = 15
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            Assert.That(async () => await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId),
                Throws.InstanceOf<ZuliValidationException>());
        }

        [Test]
        public async Task RegisterAllBaggage_ClampsCarryOnToMax2()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CarryOn = 5
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            Assert.That(async () => await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId),
                Throws.InstanceOf<ZuliValidationException>());
        }

        [Test]
        public async Task RegisterAllBaggage_WithDefaultBaggageWeight_Uses23Kg()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CheckedBaggage = 1,
                    BaggageItems = new List<BaggageItemDTO>
                    {
                        new BaggageItemDTO { Weight = 0, Size = "Mediano", Type = "Maleta" }
                    }
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggageBulk(It.Is<List<BaggageEntity>>(list =>
                list.Count == 1 &&
                list[0].Type == "Maleta" && list[0].Weight == 23m
            )), Times.Once);
        }

        [Test]
        public async Task RegisterAllBaggage_WithDefaultBaggageSize_UsesMediano()
        {
            var passengers = new List<PassengerTicketDTO>
            {
                new PassengerTicketDTO
                {
                    FirstName = "Juan",
                    FirstLastName = "Perez",
                    SecondLastName = "Garcia",
                    BirthDate = "1990-01-15",
                    PassportCountry = "Costa Rica",
                    CheckedBaggage = 1,
                    BaggageItems = new List<BaggageItemDTO>
                    {
                        new BaggageItemDTO { Weight = 20m, Size = "", Type = "Maleta" }
                    }
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggageBulk(It.Is<List<BaggageEntity>>(list =>
                list.Count == 1 &&
                list[0].Type == "Maleta" && list[0].Size == "Mediano"
            )), Times.Once);
        }

        [Test]
        public async Task CreateReservation_WithValidData_CreatesReservationAndReturnsId()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Turista",
                PaymentMethod = "card",
                ReservationOrigin = "Web"
            };
            decimal total = 500m;
            int buyerId = 10;

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(100);

            var result = await _reservationCreationService.CreateReservation(request, total, buyerId);

            Assert.That(result.ReservationId, Is.EqualTo(100));
            Assert.That(result.ReservationCode, Is.Not.Null.And.Length.EqualTo(8));

            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                !string.IsNullOrEmpty(res.ReservationCode) &&
                res.ReservationCode.Length == 8 &&
                res.BuyerId == buyerId &&
                res.TotalPayment == total &&
                res.FlightClass == "Turista" &&
                res.PaymentMethod == "card" &&
                res.ReservationOrigin == "Web"
            )), Times.Once);
        }

        [Test]
        public async Task CreateReservation_SetsPurchaseDateToToday()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Primera Clase",
                PaymentMethod = "paypal",
                ReservationOrigin = "Mobile"
            };
            decimal total = 1000m;
            int buyerId = 5;

            ReservationEntity? capturedEntity = null;
            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .Callback<ReservationEntity>((entity) => capturedEntity = entity)
                .ReturnsAsync(200);

            await _reservationCreationService.CreateReservation(request, total, buyerId);

            Assert.That(capturedEntity, Is.Not.Null);
            Assert.That(capturedEntity!.PurchaseDate, Is.EqualTo(DateTime.Now.Date));
            Assert.That(capturedEntity.ReservationCode, Is.Not.Null.And.Length.EqualTo(8));
        }

        [Test]
        public async Task CreateReservation_WithFirstClass_SetsCorrectFlightClass()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Primera Clase",
                PaymentMethod = "card",
                ReservationOrigin = "Web"
            };
            decimal total = 1500m;
            int buyerId = 3;

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(300);

            var result = await _reservationCreationService.CreateReservation(request, total, buyerId);

            Assert.That(result.ReservationCode, Is.Not.Null.And.Length.EqualTo(8));
            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                res.FlightClass == "Primera Clase" &&
                !string.IsNullOrEmpty(res.ReservationCode)
            )), Times.Once);
        }

        [Test]
        public async Task CreateReservation_PassesAllParametersCorrectly()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Turista",
                PaymentMethod = "Targeta",
                ReservationOrigin = "Agent"
            };
            decimal total = 750.50m;
            int buyerId = 99;

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(999);

            var result = await _reservationCreationService.CreateReservation(request, total, buyerId);

            Assert.That(result.ReservationId, Is.EqualTo(999));
            Assert.That(result.ReservationCode, Is.Not.Null.And.Length.EqualTo(8));

            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                !string.IsNullOrEmpty(res.ReservationCode) &&
                res.ReservationCode.Length == 8 &&
                res.BuyerId == 99 &&
                res.TotalPayment == 750.50m &&
                res.FlightClass == "Turista" &&
                res.PaymentMethod == "Targeta" &&
                res.ReservationOrigin == "Agent"
            )), Times.Once);
        }

        [Test]
        public void CalculatePurchaseBreakdown_ReturnsCorrectBaggageBreakdown()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Turista",
                Passengers = new List<PassengerTicketDTO>
                {
                    new PassengerTicketDTO
                    {
                        FirstName = "Juan",
                        FirstLastName = "Perez",
                        SecondLastName = "Garcia",
                        BirthDate = "1990-01-15",
                        PassportCountry = "Costa Rica",
                        CheckedBaggage = 2,
                        CarryOn = 1
                    },
                    new PassengerTicketDTO
                    {
                        FirstName = "Maria",
                        FirstLastName = "Lopez",
                        SecondLastName = "Sanchez",
                        BirthDate = "1985-06-20",
                        PassportCountry = "Mexico",
                        CheckedBaggage = 1,
                        CarryOn = 0
                    }
                }
            };

            var flights = new List<FlightEntity>
            {
                new FlightEntity
                {
                    Id = Guid.NewGuid(),
                    TouristPrice = 200m,
                    CheckedPrice = 50m,
                    CarryOnPrice = 25m,
                    CheckedBagMultiplier = 1.5m
                }
            };

            var breakdown = TicketPurchaseService.CalculatePurchaseBreakdown(request, flights);

            Assert.That(breakdown, Is.Not.Null);
            Assert.That(breakdown.Flights.Count, Is.EqualTo(1));
            Assert.That(breakdown.GrandTotal, Is.GreaterThan(0));

            var flightBreakdown = breakdown.Flights[0];
            Assert.That(flightBreakdown.Passengers.Count, Is.EqualTo(2));

            var passenger1 = flightBreakdown.Passengers[0];
            Assert.That(passenger1.TicketPrice, Is.EqualTo(200m));
            Assert.That(passenger1.CheckedBags.Count, Is.EqualTo(2));
            Assert.That(passenger1.CheckedBags[0].Price, Is.EqualTo(50m)); // 50 * 1.5^0 = 50
            Assert.That(passenger1.CheckedBags[1].Price, Is.EqualTo(75m)); // 50 * 1.5^1 = 75
            Assert.That(passenger1.CarryOnQuantity, Is.EqualTo(1));
            Assert.That(passenger1.CarryOnTotal, Is.EqualTo(25m));

            var passenger2 = flightBreakdown.Passengers[1];
            Assert.That(passenger2.CheckedBags.Count, Is.EqualTo(1));
            Assert.That(passenger2.CarryOnQuantity, Is.EqualTo(0));
            Assert.That(passenger2.CarryOnTotal, Is.EqualTo(0m));
        }

        [Test]
        public void CalculatePurchaseBreakdown_WithMultipleFlights_ChargesBaggagePerFlight()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Turista",
                Passengers = new List<PassengerTicketDTO>
                {
                    new PassengerTicketDTO
                    {
                        FirstName = "Juan",
                        FirstLastName = "Perez",
                        SecondLastName = "Garcia",
                        BirthDate = "1990-01-15",
                        PassportCountry = "Costa Rica",
                        CheckedBaggage = 2,
                        CarryOn = 1
                    }
                }
            };

            var flights = new List<FlightEntity>
            {
                new FlightEntity
                {
                    Id = Guid.NewGuid(),
                    TouristPrice = 200m,
                    CheckedPrice = 50m,
                    CarryOnPrice = 25m,
                    CheckedBagMultiplier = 1.5m
                },
                new FlightEntity
                {
                    Id = Guid.NewGuid(),
                    TouristPrice = 150m,
                    CheckedPrice = 40m,
                    CarryOnPrice = 20m,
                    CheckedBagMultiplier = 1.2m
                }
            };

            var breakdown = TicketPurchaseService.CalculatePurchaseBreakdown(request, flights);

            Assert.That(breakdown.Flights.Count, Is.EqualTo(2));

            var passenger1 = breakdown.Flights[0].Passengers[0];
            // Ticket 200 + bag1 50*1.5^0=50 + bag2 50*1.5^1=75 + carryOn 25 = 350
            Assert.That(passenger1.TicketPrice, Is.EqualTo(200m));
            Assert.That(passenger1.CheckedBags[0].Price, Is.EqualTo(50m));
            Assert.That(passenger1.CheckedBags[1].Price, Is.EqualTo(75m));
            Assert.That(passenger1.CarryOnTotal, Is.EqualTo(25m));
            Assert.That(passenger1.PassengerTotal, Is.EqualTo(350m));

            var passenger2 = breakdown.Flights[1].Passengers[0];
            // Ticket 150 + bag1 40*1.2^0=40 + bag2 40*1.2^1=48 + carryOn 20 = 258
            Assert.That(passenger2.TicketPrice, Is.EqualTo(150m));
            Assert.That(passenger2.CheckedBags[0].Price, Is.EqualTo(40m));
            Assert.That(passenger2.CheckedBags[1].Price, Is.EqualTo(48m));
            Assert.That(passenger2.CarryOnTotal, Is.EqualTo(20m));
            Assert.That(passenger2.PassengerTotal, Is.EqualTo(258m));

            Assert.That(breakdown.GrandTotal, Is.EqualTo(608m));
        }

        [Test]
        public void CalculatePurchaseBreakdown_WithFirstClass_UsesFirstClassPrice()
        {
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Primera Clase",
                Passengers = new List<PassengerTicketDTO>
                {
                    new PassengerTicketDTO
                    {
                        FirstName = "Ana",
                        FirstLastName = "Gomez",
                        SecondLastName = "Ruiz",
                        BirthDate = "1995-03-10",
                        PassportCountry = "Costa Rica",
                        CheckedBaggage = 0,
                        CarryOn = 0
                    }
                }
            };

            var flights = new List<FlightEntity>
            {
                new FlightEntity
                {
                    Id = Guid.NewGuid(),
                    TouristPrice = 200m,
                    FirstClassPrice = 500m,
                    CheckedPrice = 50m,
                    CarryOnPrice = 25m,
                    CheckedBagMultiplier = 1.0m
                }
            };

            var breakdown = TicketPurchaseService.CalculatePurchaseBreakdown(request, flights);

            Assert.That(breakdown.Flights[0].Passengers[0].TicketPrice, Is.EqualTo(500m));
            Assert.That(breakdown.GrandTotal, Is.EqualTo(500m));
        }

        private static TicketPurchaseRequestDTO BuildTicketPurchaseRequest(int passengerCount)
        {
            return new TicketPurchaseRequestDTO
            {
                FlightRoutes = new List<SummarizedFlightRoute>
                {
                    new SummarizedFlightRoute
                    {
                        FlightRouteId = 1,
                        DepartureDate = "2026-07-15"
                    }
                },
                Passengers = Enumerable.Range(1, passengerCount)
                    .Select(index => new PassengerTicketDTO
                    {
                        FirstName = $"Passenger{index}",
                        FirstLastName = "Test",
                        SecondLastName = "User",
                        BirthDate = "1990-01-01",
                        PassportCountry = "Costa Rica"
                    })
                    .ToList()
            };
        }
    }
}
