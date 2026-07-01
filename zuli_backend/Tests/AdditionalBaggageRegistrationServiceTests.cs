using Bogus;
using Moq;
using zuli_Business;
using zuli_Business.DTO.ReservationSearch;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class AdditionalBaggageRegistrationServiceTests
    {
        private Mock<IBaggageRepository> _baggageRepositoryMock;
        private Mock<IFlightRepository> _flightRepositoryMock;

        private BaggageRegistrationService _baggageRegistrationService;

        [SetUp]
        public void SetUp()
        {
            Randomizer.Seed = new Random(12345);

            _baggageRepositoryMock = new Mock<IBaggageRepository>();
            _flightRepositoryMock = new Mock<IFlightRepository>();

            _baggageRegistrationService = new BaggageRegistrationService(
                _baggageRepositoryMock.Object,
                _flightRepositoryMock.Object
            );
        }

        [Test]
        public async Task AddAdditionalBaggageTransactional_WithCheckedBaggage_CreatesCheckedBaggageItems()
        {
            var reservationCode = BuildReservationCode();
            var passengers = new List<AdditionalBaggagePassengerDTO>
            {
                BuildAdditionalBaggagePassengerDto(passengerId: 10, checkedBaggage: 2, carryOn: 0)
            };

            await _baggageRegistrationService.AddAdditionalBaggageTransactional(reservationCode, passengers);

            _baggageRepositoryMock.Verify(repository => repository.AddAdditionalBaggageTransactional(
                    reservationCode,
                    It.Is<List<BaggageEntity>>(baggages =>
                        baggages.Count == 2 &&
                        baggages.All(baggage =>
                            baggage.PassengerId == 10 &&
                            baggage.Weight == 23.0m &&
                            baggage.Size == "Mediano" &&
                            baggage.Type == "Maleta"
                        )
                    )
                ),
                Times.Once
            );
        }

        [Test]
        public async Task AddAdditionalBaggageTransactional_WithCarryOn_CreatesCarryOnBaggageItems()
        {
            var reservationCode = BuildReservationCode();
            var passengers = new List<AdditionalBaggagePassengerDTO>
            {
                BuildAdditionalBaggagePassengerDto(passengerId: 11, checkedBaggage: 0, carryOn: 1)
            };

            await _baggageRegistrationService.AddAdditionalBaggageTransactional(reservationCode, passengers);

            _baggageRepositoryMock.Verify(repository => repository.AddAdditionalBaggageTransactional(
                    reservationCode,
                    It.Is<List<BaggageEntity>>(baggages =>
                        baggages.Count == 1 &&
                        baggages[0].PassengerId == 11 &&
                        baggages[0].Weight == 7.0m &&
                        baggages[0].Size == "Pequeño" &&
                        baggages[0].Type == "Mano"
                    )
                ),
                Times.Once
            );
        }

        [Test]
        public async Task AddAdditionalBaggageTransactional_WithMultiplePassengers_CreatesAllBaggageItems()
        {
            var reservationCode = BuildReservationCode();
            var passengers = new List<AdditionalBaggagePassengerDTO>
            {
                BuildAdditionalBaggagePassengerDto(passengerId: 20, checkedBaggage: 2, carryOn: 1),
                BuildAdditionalBaggagePassengerDto(passengerId: 21, checkedBaggage: 1, carryOn: 0)
            };

            await _baggageRegistrationService.AddAdditionalBaggageTransactional(reservationCode, passengers);

            _baggageRepositoryMock.Verify(repository => repository.AddAdditionalBaggageTransactional(
                    reservationCode,
                    It.Is<List<BaggageEntity>>(baggages =>
                        baggages.Count == 4 &&
                        baggages.Count(baggage => baggage.PassengerId == 20 && baggage.Type == "Maleta") == 2 &&
                        baggages.Count(baggage => baggage.PassengerId == 20 && baggage.Type == "Mano") == 1 &&
                        baggages.Count(baggage => baggage.PassengerId == 21 && baggage.Type == "Maleta") == 1
                    )
                ),
                Times.Once
            );
        }

        [Test]
        public async Task AddAdditionalBaggageTransactional_NormalizesReservationCodeBeforeCallingRepository()
        {
            var reservationCode = "  ab12cd34  ";
            var passengers = new List<AdditionalBaggagePassengerDTO>
            {
                BuildAdditionalBaggagePassengerDto(passengerId: 30, checkedBaggage: 1, carryOn: 0)
            };

            await _baggageRegistrationService.AddAdditionalBaggageTransactional(reservationCode, passengers);

            _baggageRepositoryMock.Verify(repository => repository.AddAdditionalBaggageTransactional(
                    "AB12CD34",
                    It.IsAny<List<BaggageEntity>>()
                ),
                Times.Once
            );
        }

        [Test]
        public void AddAdditionalBaggageTransactional_WithNoAdditionalBaggage_ThrowsZuliValidationException()
        {
            var reservationCode = BuildReservationCode();
            var passengers = new List<AdditionalBaggagePassengerDTO>
            {
                BuildAdditionalBaggagePassengerDto(passengerId: 40, checkedBaggage: 0, carryOn: 0)
            };

            Assert.That(
                async () => await _baggageRegistrationService.AddAdditionalBaggageTransactional(reservationCode, passengers),
                Throws.TypeOf<ZuliValidationException>()
            );

            _baggageRepositoryMock.Verify(repository => repository.AddAdditionalBaggageTransactional(
                    It.IsAny<string>(),
                    It.IsAny<List<BaggageEntity>>()
                ),
                Times.Never
            );
        }

        [Test]
        public void AddAdditionalBaggageTransactional_WithEmptyPassengerList_ThrowsZuliValidationException()
        {
            var reservationCode = BuildReservationCode();
            var passengers = new List<AdditionalBaggagePassengerDTO>();

            Assert.That(
                async () => await _baggageRegistrationService.AddAdditionalBaggageTransactional(reservationCode, passengers),
                Throws.TypeOf<ZuliValidationException>()
            );

            _baggageRepositoryMock.Verify(repository => repository.AddAdditionalBaggageTransactional(
                    It.IsAny<string>(),
                    It.IsAny<List<BaggageEntity>>()
                ),
                Times.Never
            );
        }

        private static string BuildReservationCode()
        {
            return new Faker().Random.AlphaNumeric(8).ToUpperInvariant();
        }

        private static AdditionalBaggagePassengerDTO BuildAdditionalBaggagePassengerDto(
            int passengerId,
            int checkedBaggage,
            int carryOn)
        {
            return new Faker<AdditionalBaggagePassengerDTO>()
                .RuleFor(passenger => passenger.PassengerId, _ => passengerId)
                .RuleFor(passenger => passenger.AdditionalCheckedBaggage, _ => checkedBaggage)
                .RuleFor(passenger => passenger.AdditionalCarryOn, _ => carryOn)
                .Generate();
        }
    }
}
