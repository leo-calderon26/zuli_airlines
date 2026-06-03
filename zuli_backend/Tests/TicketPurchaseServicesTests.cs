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

        private PassengerValidationService _passengerValidationService;
        private BaggageRegistrationService _baggageRegistrationService;
        private ReservationCreationService _reservationCreationService;

        [SetUp]
        public void SetUp()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _baggageRepoMock = new Mock<IBaggageRepository>();

            _passengerValidationService = new PassengerValidationService(_reservationRepoMock.Object);
            _baggageRegistrationService = new BaggageRegistrationService(_baggageRepoMock.Object);
            _reservationCreationService = new ReservationCreationService(_reservationRepoMock.Object);
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

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b =>
                b.PassengerId == 1 &&
                b.ReservationId == reservationId &&
                b.Type == "Maleta" &&
                b.Weight == 25m &&
                b.Size == "Grande"
            )), Times.Once);

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b =>
                b.PassengerId == 1 &&
                b.ReservationId == reservationId &&
                b.Type == "Maleta" &&
                b.Weight == 20m &&
                b.Size == "Mediano"
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

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.Type == "Maleta")), Times.Never);
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
                    CarryOn = 2
                }
            };
            var passengerIds = new List<int> { 1 };
            var reservationId = 100;

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b =>
                b.PassengerId == 1 &&
                b.ReservationId == reservationId &&
                b.Type == "Mano" &&
                b.Weight == 7m &&
                b.Size == "Pequeño"
            )), Times.Exactly(2));
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

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.PassengerId == 1)), Times.Exactly(2));
            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.PassengerId == 2)), Times.Exactly(2));
        }

        [Test]
        public async Task RegisterAllBaggage_ClampsCheckedBaggageToMax10()
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

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.Type == "Maleta")), Times.Exactly(10));
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

            await _baggageRegistrationService.RegisterAllBaggage(passengers, passengerIds, reservationId);

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b => b.Type == "Mano")), Times.Exactly(2));
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

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b =>
                b.Type == "Maleta" && b.Weight == 23m
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

            _baggageRepoMock.Verify(r => r.CreateBaggage(It.Is<BaggageEntity>(b =>
                b.Type == "Maleta" && b.Size == "Mediano"
            )), Times.Once);
        }

        [Test]
        public async Task CreateReservation_WithValidData_CreatesReservationAndReturnsId()
        {
            var code = "ABC12345";
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

            var result = await _reservationCreationService.CreateReservation(code, request, total, buyerId);

            Assert.That(result, Is.EqualTo(100));

            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                res.ReservationCode == code &&
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
            var code = "XYZ99999";
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
                .Callback<ReservationEntity>(entity => capturedEntity = entity)
                .ReturnsAsync(200);

            await _reservationCreationService.CreateReservation(code, request, total, buyerId);

            Assert.That(capturedEntity, Is.Not.Null);
            Assert.That(capturedEntity!.PurchaseDate, Is.EqualTo(DateTime.Now.Date));
        }

        [Test]
        public async Task CreateReservation_WithFirstClass_SetsCorrectFlightClass()
        {
            var code = "FIRST01";
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

            await _reservationCreationService.CreateReservation(code, request, total, buyerId);

            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                res.FlightClass == "Primera Clase"
            )), Times.Once);
        }

        [Test]
        public async Task CreateReservation_PassesAllParametersCorrectly()
        {
            var code = "TESTCODE";
            var request = new TicketPurchaseRequestDTO
            {
                FlightClass = "Turista",
                PaymentMethod = "crypto",
                ReservationOrigin = "Agent"
            };
            decimal total = 750.50m;
            int buyerId = 99;

            _reservationRepoMock
                .Setup(r => r.CreateReservation(It.IsAny<ReservationEntity>()))
                .ReturnsAsync(999);

            var result = await _reservationCreationService.CreateReservation(code, request, total, buyerId);

            _reservationRepoMock.Verify(r => r.CreateReservation(It.Is<ReservationEntity>(res =>
                res.ReservationCode == "TESTCODE" &&
                res.BuyerId == 99 &&
                res.TotalPayment == 750.50m &&
                res.FlightClass == "Turista" &&
                res.PaymentMethod == "Targeta" &&
                res.ReservationOrigin == "Agent"
            )), Times.Once);
        }
    }
}