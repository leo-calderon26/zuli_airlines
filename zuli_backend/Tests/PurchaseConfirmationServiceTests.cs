using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class PurchaseConfirmationServiceTests
    {
        private Mock<IPurchaseConfirmationRepository> _purchaseConfirmationRepositoryMock;
        private Mock<IPurchaseEmailService> _purchaseEmailServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<PurchaseConfirmationPageDTO>> _purchaseConfirmationValidatorMock;

        private PurchaseConfirmationService _purchaseConfirmationService;

        [SetUp]
        public void SetUp()
        {
            _purchaseConfirmationRepositoryMock = new Mock<IPurchaseConfirmationRepository>();
            _purchaseEmailServiceMock = new Mock<IPurchaseEmailService>();
            _mapperMock = new Mock<IMapper>();
            _purchaseConfirmationValidatorMock = new Mock<IValidator<PurchaseConfirmationPageDTO>>();

            _purchaseConfirmationService = new PurchaseConfirmationService(
                _purchaseConfirmationRepositoryMock.Object,
                _purchaseEmailServiceMock.Object,
                _mapperMock.Object,
                _purchaseConfirmationValidatorMock.Object
            );
        }

        [Test]
        public async Task GetConfirmationPageAsync_ExistingReservation_ReturnsConfirmationData()
        {
            var reservationCode = "ZUTEST001";
            var confirmationEntity = BuildValidConfirmationEntity();
            var confirmationDto = BuildValidConfirmationDto();

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);

            var result = await _purchaseConfirmationService.GetConfirmationPageAsync(reservationCode);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReservationId, Is.EqualTo(100));
            Assert.That(result.ReservationCode, Is.EqualTo(reservationCode));
            Assert.That(result.BuyerEmail, Is.EqualTo("buyer@test.com"));
            Assert.That(result.Passengers.Count, Is.EqualTo(1));
            Assert.That(result.Flights.Count, Is.EqualTo(1));

            _purchaseConfirmationRepositoryMock.Verify(
                repository => repository.GetPurchaseConfirmationAsync(reservationCode),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity),
                Times.Once
            );

            _purchaseConfirmationValidatorMock.Verify(
                validator => validator.Validate(It.IsAny<PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync(
                    It.IsAny<PurchaseConfirmationPageDTO>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public void GetConfirmationPageAsync_ReservationDoesNotExist_ThrowsZuliNotFoundException()
        {
            var reservationCode = "INVALID01";

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            Assert.That(
                async () => await _purchaseConfirmationService.GetConfirmationPageAsync(reservationCode),
                Throws.TypeOf<ZuliNotFoundException>()
            );

            _purchaseConfirmationRepositoryMock.Verify(
                repository => repository.GetPurchaseConfirmationAsync(reservationCode),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<PurchaseConfirmationPageDTO>(It.IsAny<PurchaseConfirmationEntity>()),
                Times.Never
            );

            _purchaseConfirmationValidatorMock.Verify(
                validator => validator.Validate(It.IsAny<PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync(
                    It.IsAny<PurchaseConfirmationPageDTO>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task CompleteConfirmationAsync_ValidReservation_GeneratesPdfsAndSendsEmails()
        {
            var reservationCode = "ZUTEST001";
            var confirmationEntity = BuildValidConfirmationEntity();
            var confirmationDto = BuildValidConfirmationDto();
            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);

            _purchaseConfirmationValidatorMock
                .Setup(validator => validator.Validate(confirmationDto))
                .Returns(new ValidationResult());

            _purchaseEmailServiceMock
                .Setup(service => service.SendPurchaseEmailsAsync(
                    confirmationDto,
                    It.IsAny<CancellationToken>()
                ))
                .Returns(Task.CompletedTask);

            var result = await _purchaseConfirmationService.CompleteConfirmationAsync(reservationCode);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReservationId, Is.EqualTo(100));
            Assert.That(result.ReservationCode, Is.EqualTo(reservationCode));
            Assert.That(result.InvoiceEmailSent, Is.True);
            Assert.That(result.ConfirmationEmailSent, Is.True);
            Assert.That(result.EmailsSent, Is.True);
            Assert.That(
                result.Message,
                Is.EqualTo("Reserva completada. Los detalles han sido enviados a su correo electrónico.")
            );

            _purchaseConfirmationRepositoryMock.Verify(
                repository => repository.GetPurchaseConfirmationAsync(reservationCode),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity),
                Times.Once
            );

            _purchaseConfirmationValidatorMock.Verify(
                validator => validator.Validate(confirmationDto),
                Times.Once
            );

            _purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync(
                    confirmationDto,
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task GetConfirmationPageAsync_ReturnsBreakdownWithBaggagePrices()
        {
            var reservationCode = "ZUTEST001";
            var confirmationEntity = BuildValidConfirmationEntity();
            var confirmationDto = BuildValidConfirmationDto();
        
            confirmationDto.Passengers[0].CheckedBaggageQuantity = 3;
        
            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);
        
            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);
        
            var result = await _purchaseConfirmationService.GetConfirmationPageAsync(reservationCode);
        
            var passengerBreakdown = result.Breakdown.Flights[0].Passengers[0];
        
            Assert.That(result.Breakdown, Is.Not.Null);
            Assert.That(result.Breakdown.Flights.Count, Is.EqualTo(1));
            Assert.That(result.Breakdown.Flights[0].Passengers.Count, Is.EqualTo(1));
        
            Assert.That(passengerBreakdown.TicketPrice, Is.EqualTo(200m));
            Assert.That(passengerBreakdown.CheckedBags.Count, Is.EqualTo(3));
        
            Assert.That(passengerBreakdown.CheckedBags[0].Price, Is.EqualTo(50m));
            Assert.That(passengerBreakdown.CheckedBags[1].Price, Is.EqualTo(75m));
            Assert.That(passengerBreakdown.CheckedBags[2].Price, Is.EqualTo(112.5m));
        
            Assert.That(passengerBreakdown.CarryOnTotal, Is.EqualTo(25m));
            Assert.That(passengerBreakdown.PassengerTotal, Is.EqualTo(462.5m));
        
            Assert.That(result.Breakdown.Flights[0].FlightTotal, Is.EqualTo(462.5m));
            Assert.That(result.Breakdown.GrandTotal, Is.EqualTo(462.5m));
        }

        [Test]
        public void CompleteConfirmationAsync_ReservationDoesNotExist_ThrowsZuliNotFoundException()
        {
            var reservationCode = "INVALID01";

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            Assert.That(
                async () => await _purchaseConfirmationService.CompleteConfirmationAsync(reservationCode),
                Throws.TypeOf<ZuliNotFoundException>()
            );

            _mapperMock.Verify(
                mapper => mapper.Map<PurchaseConfirmationPageDTO>(It.IsAny<PurchaseConfirmationEntity>()),
                Times.Never
            );

            _purchaseConfirmationValidatorMock.Verify(
                validator => validator.Validate(It.IsAny<PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync(
                    It.IsAny<PurchaseConfirmationPageDTO>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public void CompleteConfirmationAsync_InvalidConfirmationData_ThrowsZuliValidationException()
        {
            var reservationCode = "ZUTEST001";
            var confirmationEntity = BuildValidConfirmationEntity();
            var confirmationDto = BuildValidConfirmationDto();

            var validationResult = new ValidationResult(
                new List<ValidationFailure>
                {
                    new ValidationFailure(
                        "buyerEmail",
                        "La reserva no tiene correo del comprador."
                    )
                }
            );

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);

            _purchaseConfirmationValidatorMock
                .Setup(validator => validator.Validate(confirmationDto))
                .Returns(validationResult);

            Assert.That(
                async () => await _purchaseConfirmationService.CompleteConfirmationAsync(reservationCode),
                Throws.TypeOf<ZuliValidationException>()
            );

            _purchaseConfirmationValidatorMock.Verify(
                validator => validator.Validate(confirmationDto),
                Times.Once
            );

            _purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync(
                    It.IsAny<PurchaseConfirmationPageDTO>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public void CompleteConfirmationAsync_EmailFails_ThrowsZuliEmailException()
        {
            var reservationCode = "ZUTEST001";
            var confirmationEntity = BuildValidConfirmationEntity();
            var confirmationDto = BuildValidConfirmationDto();
            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);

            _purchaseConfirmationValidatorMock
                .Setup(validator => validator.Validate(confirmationDto))
                .Returns(new ValidationResult());

            _purchaseEmailServiceMock
                .Setup(service => service.SendPurchaseEmailsAsync(
                    confirmationDto,
                    It.IsAny<CancellationToken>()
                ))
                .ThrowsAsync(new Exception("SMTP error"));

            Assert.That(
                async () => await _purchaseConfirmationService.CompleteConfirmationAsync(reservationCode),
                Throws.TypeOf<ZuliEmailException>()
            );

            _purchaseConfirmationValidatorMock.Verify(
                validator => validator.Validate(confirmationDto),
                Times.Once
            );

            _purchaseEmailServiceMock.Verify(
                service => service.SendPurchaseEmailsAsync(
                    confirmationDto,
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        private static PurchaseConfirmationEntity BuildValidConfirmationEntity()
        {
            return new PurchaseConfirmationEntity
            {
                ReservationId = 100,
                ReservationCode = "ZUTEST001",
                BuyerName = "Valeria Jimenez Castro",
                BuyerEmail = "buyer@test.com",
                BuyerPhone = "+506 8888 4721",
                PaymentMethod = "TARJETA BANCARIA",
                FlightClass = "Económica",
                TotalAmount = 865m,
                Passengers = new List<PurchaseConfirmationPassengerEntity>
                {
                    new PurchaseConfirmationPassengerEntity
                    {
                        FullName = "Mateo Rodriguez Vega",
                        BirthDate = "1996-03-22",
                        Gender = "Masculino",
                        PassportCountry = "Costa Rica",
                        CheckedBaggageQuantity = 1,
                        CarryOnQuantity = 1
                    }
                },
                Flights = new List<PurchaseConfirmationFlightEntity>
                {
                    new PurchaseConfirmationFlightEntity
                    {
                        FlightId = Guid.NewGuid(),
                        FlightNumber = "ZU-TEST",
                        AirlineName = "zuliAirline",
                        OriginAirportName = "Juan Santamaría International Airport",
                        OriginAirportCode = "SJO",
                        DestinationAirportName = "Tocumen International Airport",
                        DestinationAirportCode = "PTY",
                        DepartureDateTime = new DateTime(2026, 8, 18, 9, 20, 0),
                        ArrivalDateTime = new DateTime(2026, 8, 18, 10, 45, 0),
                        CheckedPrice = 50m,
                        CarryOnPrice = 25m,
                        CheckedBagMultiplier = 1.5m,
                        TouristPrice = 200m,
                        FirstClassPrice = 500m
                    }
                }
            };
        }

        private static PurchaseConfirmationPageDTO BuildValidConfirmationDto()
        {
            return new PurchaseConfirmationPageDTO
            {
                ReservationId = 100,
                ReservationCode = "ZUTEST001",
                BuyerName = "Valeria Jimenez Castro",
                BuyerEmail = "buyer@test.com",
                BuyerPhone = "+506 8888 4721",
                PaymentMethod = "TARJETA BANCARIA",
                FlightClass = "Económica",
                TotalAmount = 865m,
                Passengers = new List<PurchaseConfirmationPassengerDTO>
                {
                    new PurchaseConfirmationPassengerDTO
                    {
                        FullName = "Mateo Rodriguez Vega",
                        BirthDate = "1996-03-22",
                        Gender = "Masculino",
                        PassportCountry = "Costa Rica",
                        CheckedBaggageQuantity = 1,
                        CarryOnQuantity = 1
                    }
                },
                Flights = new List<PurchaseConfirmationFlightDTO>
                {
                    new PurchaseConfirmationFlightDTO
                    {
                        FlightId = Guid.NewGuid(),
                        FlightNumber = "ZU-TEST",
                        AirlineName = "zuliAirline",
                        OriginAirportName = "Juan Santamaría International Airport",
                        OriginAirportCode = "SJO",
                        DestinationAirportName = "Tocumen International Airport",
                        DestinationAirportCode = "PTY",
                        DepartureDateTime = new DateTime(2026, 8, 18, 9, 20, 0),
                        ArrivalDateTime = new DateTime(2026, 8, 18, 10, 45, 0),
                        CheckedPrice = 50m,
                        CarryOnPrice = 25m,
                        CheckedBagMultiplier = 1.5m,
                        TouristPrice = 200m,
                        FirstClassPrice = 500m
                    }
                }
            };
        }
    }
}
