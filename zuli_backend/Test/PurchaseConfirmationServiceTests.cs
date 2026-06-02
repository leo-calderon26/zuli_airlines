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
        private Mock<IPurchaseConfirmationPdfService> _purchaseConfirmationPdfServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IMapper> _mapperMock;

        private PurchaseConfirmationService _purchaseConfirmationService;

        [SetUp]
        public void SetUp()
        {
            _purchaseConfirmationRepositoryMock = new Mock<IPurchaseConfirmationRepository>();
            _purchaseConfirmationPdfServiceMock = new Mock<IPurchaseConfirmationPdfService>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();

            _purchaseConfirmationService = new PurchaseConfirmationService(
                _purchaseConfirmationRepositoryMock.Object,
                _purchaseConfirmationPdfServiceMock.Object,
                _emailServiceMock.Object,
                _mapperMock.Object
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
            Assert.That(result!.ReservationId, Is.EqualTo(100));
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

            _purchaseConfirmationPdfServiceMock.Verify(
                pdfService => pdfService.GenerateInvoicePdf(It.IsAny<PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendInvoiceEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task GetConfirmationPageAsync_ReservationDoesNotExist_ReturnsNull()
        {
            var reservationCode = "INVALID01";

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            var result = await _purchaseConfirmationService.GetConfirmationPageAsync(reservationCode);

            Assert.That(result, Is.Null);

            _purchaseConfirmationRepositoryMock.Verify(
                repository => repository.GetPurchaseConfirmationAsync(reservationCode),
                Times.Once
            );

            _mapperMock.Verify(
                mapper => mapper.Map<PurchaseConfirmationPageDTO>(It.IsAny<PurchaseConfirmationEntity>()),
                Times.Never
            );

            _purchaseConfirmationPdfServiceMock.Verify(
                pdfService => pdfService.GenerateInvoicePdf(It.IsAny<PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendPurchaseConfirmationEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
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
            var invoicePdf = new byte[] { 1, 2, 3 };
            var confirmationPdf = new byte[] { 4, 5, 6 };

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);

            _purchaseConfirmationPdfServiceMock
                .Setup(pdfService => pdfService.GenerateInvoicePdf(confirmationDto))
                .Returns(invoicePdf);

            _purchaseConfirmationPdfServiceMock
                .Setup(pdfService => pdfService.GenerateConfirmationPdf(confirmationDto))
                .Returns(confirmationPdf);

            _emailServiceMock
                .Setup(emailService => emailService.SendInvoiceEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ))
                .Returns(Task.CompletedTask);

            _emailServiceMock
                .Setup(emailService => emailService.SendPurchaseConfirmationEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
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

            _purchaseConfirmationPdfServiceMock.Verify(
                pdfService => pdfService.GenerateInvoicePdf(confirmationDto),
                Times.Once
            );

            _purchaseConfirmationPdfServiceMock.Verify(
                pdfService => pdfService.GenerateConfirmationPdf(confirmationDto),
                Times.Once
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendInvoiceEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    reservationCode,
                    invoicePdf
                ),
                Times.Once
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendPurchaseConfirmationEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    reservationCode,
                    confirmationPdf
                ),
                Times.Once
            );
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

            _purchaseConfirmationPdfServiceMock.Verify(
                pdfService => pdfService.GenerateInvoicePdf(It.IsAny<PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendInvoiceEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
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
            var invoicePdf = new byte[] { 1, 2, 3 };
            var confirmationPdf = new byte[] { 4, 5, 6 };

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync(confirmationEntity);

            _mapperMock
                .Setup(mapper => mapper.Map<PurchaseConfirmationPageDTO>(confirmationEntity))
                .Returns(confirmationDto);

            _purchaseConfirmationPdfServiceMock
                .Setup(pdfService => pdfService.GenerateInvoicePdf(confirmationDto))
                .Returns(invoicePdf);

            _purchaseConfirmationPdfServiceMock
                .Setup(pdfService => pdfService.GenerateConfirmationPdf(confirmationDto))
                .Returns(confirmationPdf);

            _emailServiceMock
                .Setup(emailService => emailService.SendInvoiceEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ))
                .ThrowsAsync(new Exception("SMTP error"));

            Assert.That(
                async () => await _purchaseConfirmationService.CompleteConfirmationAsync(reservationCode),
                Throws.TypeOf<ZuliEmailException>()
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendInvoiceEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    reservationCode,
                    invoicePdf
                ),
                Times.Once
            );

            _emailServiceMock.Verify(
                emailService => emailService.SendPurchaseConfirmationEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ),
                Times.Never
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
                        ArrivalDateTime = new DateTime(2026, 8, 18, 10, 45, 0)
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
                        ArrivalDateTime = new DateTime(2026, 8, 18, 10, 45, 0)
                    }
                }
            };
        }
    }
}