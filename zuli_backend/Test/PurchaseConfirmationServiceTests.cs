using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Business;
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

        private PurchaseConfirmationService _purchaseConfirmationService;

        [SetUp]
        public void SetUp()
        {
            _purchaseConfirmationRepositoryMock = new Mock<IPurchaseConfirmationRepository>();
            _purchaseConfirmationPdfServiceMock = new Mock<IPurchaseConfirmationPdfService>();
            _emailServiceMock = new Mock<IEmailService>();

            _purchaseConfirmationService = new PurchaseConfirmationService(
                _purchaseConfirmationRepositoryMock.Object,
                _purchaseConfirmationPdfServiceMock.Object,
                _emailServiceMock.Object
            );
        }

        [Test]
        public async Task GetConfirmationPageAsync_ExistingReservation_ReturnsConfirmationData()
        {
            var reservationId = 100;
            var confirmationEntity = BuildValidConfirmationEntity(reservationId);

            _purchaseConfirmationRepositoryMock
                .Setup(r => r.GetPurchaseConfirmationAsync(reservationId))
                .ReturnsAsync(confirmationEntity);

            var result = await _purchaseConfirmationService.GetConfirmationPageAsync(reservationId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ReservationId, Is.EqualTo(reservationId));
            Assert.That(result.ReservationCode, Is.EqualTo("ZUTEST001"));
            Assert.That(result.BuyerEmail, Is.EqualTo("buyer@test.com"));
            Assert.That(result.Passengers.Count, Is.EqualTo(1));
            Assert.That(result.Flights.Count, Is.EqualTo(1));

            _purchaseConfirmationRepositoryMock.Verify(
                r => r.GetPurchaseConfirmationAsync(reservationId),
                Times.Once
            );

            _purchaseConfirmationPdfServiceMock.Verify(
                p => p.GenerateInvoicePdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _emailServiceMock.Verify(
                e => e.SendInvoiceEmailAsync(
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
            var reservationId = 999;

            _purchaseConfirmationRepositoryMock
                .Setup(r => r.GetPurchaseConfirmationAsync(reservationId))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            var result = await _purchaseConfirmationService.GetConfirmationPageAsync(reservationId);

            Assert.That(result, Is.Null);

            _purchaseConfirmationRepositoryMock.Verify(
                r => r.GetPurchaseConfirmationAsync(reservationId),
                Times.Once
            );

            _purchaseConfirmationPdfServiceMock.Verify(
                p => p.GenerateInvoicePdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _emailServiceMock.Verify(
                e => e.SendPurchaseConfirmationEmailAsync(
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
            var reservationId = 100;
            var confirmationEntity = BuildValidConfirmationEntity(reservationId);
            var invoicePdf = new byte[] { 1, 2, 3 };
            var confirmationPdf = new byte[] { 4, 5, 6 };

            _purchaseConfirmationRepositoryMock
                .Setup(r => r.GetPurchaseConfirmationAsync(reservationId))
                .ReturnsAsync(confirmationEntity);

            _purchaseConfirmationPdfServiceMock
                .Setup(p => p.GenerateInvoicePdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()))
                .Returns(invoicePdf);

            _purchaseConfirmationPdfServiceMock
                .Setup(p => p.GenerateConfirmationPdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()))
                .Returns(confirmationPdf);

            _emailServiceMock
                .Setup(e => e.SendInvoiceEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ))
                .Returns(Task.CompletedTask);

            _emailServiceMock
                .Setup(e => e.SendPurchaseConfirmationEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ))
                .Returns(Task.CompletedTask);

            var result = await _purchaseConfirmationService.CompleteConfirmationAsync(reservationId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReservationId, Is.EqualTo(reservationId));
            Assert.That(result.InvoiceEmailSent, Is.True);
            Assert.That(result.ConfirmationEmailSent, Is.True);
            Assert.That(result.EmailsSent, Is.True);
            Assert.That(result.Message, Is.EqualTo("Reserva completada. Los detalles han sido enviados a su correo electrónico."));

            _purchaseConfirmationPdfServiceMock.Verify(
                p => p.GenerateInvoicePdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()),
                Times.Once
            );

            _purchaseConfirmationPdfServiceMock.Verify(
                p => p.GenerateConfirmationPdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()),
                Times.Once
            );

            _emailServiceMock.Verify(
                e => e.SendInvoiceEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    "ZUTEST001",
                    invoicePdf
                ),
                Times.Once
            );

            _emailServiceMock.Verify(
                e => e.SendPurchaseConfirmationEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    "ZUTEST001",
                    confirmationPdf
                ),
                Times.Once
            );
        }

        [Test]
        public void CompleteConfirmationAsync_ReservationDoesNotExist_ThrowsZuliNotFoundException()
        {
            var reservationId = 999;

            _purchaseConfirmationRepositoryMock
                .Setup(r => r.GetPurchaseConfirmationAsync(reservationId))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            Assert.That(
                async () => await _purchaseConfirmationService.CompleteConfirmationAsync(reservationId),
                Throws.TypeOf<ZuliNotFoundException>()
            );

            _purchaseConfirmationPdfServiceMock.Verify(
                p => p.GenerateInvoicePdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()),
                Times.Never
            );

            _emailServiceMock.Verify(
                e => e.SendInvoiceEmailAsync(
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
            var reservationId = 100;
            var confirmationEntity = BuildValidConfirmationEntity(reservationId);
            var invoicePdf = new byte[] { 1, 2, 3 };
            var confirmationPdf = new byte[] { 4, 5, 6 };

            _purchaseConfirmationRepositoryMock
                .Setup(r => r.GetPurchaseConfirmationAsync(reservationId))
                .ReturnsAsync(confirmationEntity);

            _purchaseConfirmationPdfServiceMock
                .Setup(p => p.GenerateInvoicePdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()))
                .Returns(invoicePdf);

            _purchaseConfirmationPdfServiceMock
                .Setup(p => p.GenerateConfirmationPdf(It.IsAny<zuli_Business.DTO.PurchaseConfirmationPageDTO>()))
                .Returns(confirmationPdf);

            _emailServiceMock
                .Setup(e => e.SendInvoiceEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ))
                .ThrowsAsync(new Exception("SMTP error"));

            Assert.That(
                async () => await _purchaseConfirmationService.CompleteConfirmationAsync(reservationId),
                Throws.TypeOf<ZuliEmailException>()
            );

            _emailServiceMock.Verify(
                e => e.SendInvoiceEmailAsync(
                    "buyer@test.com",
                    "Valeria Jimenez Castro",
                    "ZUTEST001",
                    invoicePdf
                ),
                Times.Once
            );

            _emailServiceMock.Verify(
                e => e.SendPurchaseConfirmationEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()
                ),
                Times.Never
            );
        }

        private static PurchaseConfirmationEntity BuildValidConfirmationEntity(int reservationId)
        {
            return new PurchaseConfirmationEntity
            {
                ReservationId = reservationId,
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
    }
}