using MapsterMapper;
using Moq;
using zuli_Business;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_backend.Test
{
    [TestFixture]
    public class PurchaseEmailServiceTests
    {
        private Mock<IPurchaseConfirmationRepository> _purchaseConfirmationRepositoryMock;
        private Mock<IPurchaseConfirmationPdfService> _purchaseConfirmationPdfServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IMapper> _mapperMock;
        private PurchaseEmailService _purchaseEmailService;

        [SetUp]
        public void SetUp()
        {
            _purchaseConfirmationRepositoryMock = new Mock<IPurchaseConfirmationRepository>();
            _purchaseConfirmationPdfServiceMock = new Mock<IPurchaseConfirmationPdfService>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();

            _purchaseEmailService = new PurchaseEmailService(
                _purchaseConfirmationRepositoryMock.Object,
                _purchaseConfirmationPdfServiceMock.Object,
                _emailServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task SendPurchaseEmailsAsync_ValidReservation_GeneratesPdfsAndSendsEmails()
        {
            var reservationCode = "ZUTEST01";
            var confirmationEntity = BuildConfirmationEntity(reservationCode);
            var confirmationDto = BuildConfirmationDto(reservationCode);
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

            await _purchaseEmailService.SendPurchaseEmailsAsync(reservationCode);

            _purchaseConfirmationRepositoryMock.Verify(
                repository => repository.GetPurchaseConfirmationAsync(reservationCode),
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
        public void SendPurchaseEmailsAsync_ReservationDoesNotExist_ThrowsZuliNotFoundException()
        {
            var reservationCode = "INVALID1";

            _purchaseConfirmationRepositoryMock
                .Setup(repository => repository.GetPurchaseConfirmationAsync(reservationCode))
                .ReturnsAsync((PurchaseConfirmationEntity?)null);

            Assert.That(
                async () => await _purchaseEmailService.SendPurchaseEmailsAsync(reservationCode),
                Throws.TypeOf<ZuliNotFoundException>()
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

        private static PurchaseConfirmationEntity BuildConfirmationEntity(string reservationCode)
        {
            return new PurchaseConfirmationEntity
            {
                ReservationId = 100,
                ReservationCode = reservationCode,
                BuyerName = "Valeria Jimenez Castro",
                BuyerEmail = "buyer@test.com",
                BuyerPhone = "+506 8888 4721",
                PaymentMethod = "TARJETA BANCARIA",
                FlightClass = "Económica",
                TotalAmount = 865m
            };
        }

        private static PurchaseConfirmationPageDTO BuildConfirmationDto(string reservationCode)
        {
            return new PurchaseConfirmationPageDTO
            {
                ReservationId = 100,
                ReservationCode = reservationCode,
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
                        PassengerId = 1,
                        FullName = "Valeria Jimenez Castro",
                        BirthDate = "1990-01-01",
                        Gender = "F",
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
                        FlightNumber = "ZL-100",
                        AirlineName = "Zuli Airlines",
                        OriginAirportName = "Juan Santamaria",
                        OriginAirportCode = "SJO",
                        DestinationAirportName = "Miami International",
                        DestinationAirportCode = "MIA",
                        DepartureDateTime = DateTime.Now.AddDays(1),
                        ArrivalDateTime = DateTime.Now.AddDays(1).AddHours(3),
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
