using NUnit.Framework;
using Moq;
using System;
using QuestPDF.Infrastructure;
using zuli_Business;
using zuli_Business.DTO.ReservationSearch;
using zuli_Business.Interface;

namespace zuli_backend.Tests
{
    [TestFixture]
    public class ReservationItineraryPdfServiceTests
    {
        private Mock<IQrCodeService> _qrCodeServiceMock;
        private ReservationItineraryPdfService _pdfService;

        [SetUp]
        public void SetUp()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            _qrCodeServiceMock = new Mock<IQrCodeService>();
            
            byte[] validPngBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");
            _qrCodeServiceMock.Setup(q => q.Generate(It.IsAny<string>())).Returns(validPngBytes);

            _pdfService = new ReservationItineraryPdfService(_qrCodeServiceMock.Object);
        }

        [Test]
        public void GenerateItineraryPdf_ValidReservation_ReturnsPdfBytes()
        {
            var reservationDto = new ReservationSearchResponseDTO
            {
                ReservationCode = "ZULI1234",
                ContactEmail = "test@test.com",
                Passengers = [
                    new()
                    { 
                        FirstName = "Juan", FirstLastName = "Perez", CheckedBaggageQuantity = 1, CarryOnQuantity = 1 
                    }
                ],
                Journey = new()
                {
                    OriginCity = "San Jose", OriginCode = "SJO",
                    DestinationCity = "Panama", DestinationCode = "PTY",
                    Stops = 0,
                    FlightClass = "Turista",
                    Segments = [
                        new()
                        {
                            Airline = "Zuli Airlines",
                            FlightNumber = "ZU-123",
                            OriginCode = "SJO",
                            DestinationCode = "PTY",
                            DepartureDateTime = DateTime.Now.AddDays(1),
                            ArrivalDateTime = DateTime.Now.AddDays(1).AddHours(2),
                            AircraftModel = "Boeing 737"
                        }
                    ]
                }
            };

            var result = _pdfService.GenerateItineraryPdf(reservationDto);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Length.GreaterThan(0));
                
                // Validación de que los primeros bytes corresponden a un archivo PDF válido
                Assert.That(result[0], Is.EqualTo(0x25));
                Assert.That(result[1], Is.EqualTo(0x50));
                Assert.That(result[2], Is.EqualTo(0x44));
                Assert.That(result[3], Is.EqualTo(0x46));
            });

            _qrCodeServiceMock.Verify(q => q.Generate(It.IsAny<string>()), Times.Once);
        }
    }

    [TestFixture]
    public class QrCodeServiceTests
    {
        [Test]
        public void Generate_ValidString_ReturnsPngBytes()
        {
            var service = new QrCodeService();
            string testData = "TestData-12345";

            var result = service.Generate(testData);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Length.GreaterThan(0));

                // Validación de que los primeros bytes corresponden a un archivo PNG válido
                Assert.That(result[0], Is.EqualTo(137));
                Assert.That(result[1], Is.EqualTo(80));
                Assert.That(result[2], Is.EqualTo(78));
                Assert.That(result[3], Is.EqualTo(71));
            });
        }
    }
}