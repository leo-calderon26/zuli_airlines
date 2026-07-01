using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;
using zuli_Business.DTO.ReservationSearch;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class ReservationItineraryPdfService : IReservationItineraryPdfService
    {
        private readonly IQrCodeService _qrCodeService;

        private const string PRIMARY_COLOR = "#7A0C12";
        private const string HEADER_BG_COLOR = "#f3efef";
        private const int PAGE_MARGIN = 40;
        private const int DEFAULT_FONT_SIZE = 10;
        private const int TITLE_FONT_SIZE = 24;
        private const int SUBTITLE_FONT_SIZE = 14;
        private const int SECTION_PADDING = 20;
        private const int CELL_PADDING = 5;

        private const string DEFAULT_AIRLINE_NAME = "Zuli Airlines";
        private const string DOC_TITLE = "Itinerario de Viaje y Pases de Abordar";
        private const string LBL_RESERVATION = "Reserva";
        private const string LBL_CONTACT = "Contacto:";
        private const string SEC_PASSENGERS = "Datos de Pasajeros y Equipaje";
        private const string SEC_ITINERARY = "Detalles del Itinerario";
        private const string SEC_BOARDING_PASSES = "Pases de Abordar";
        private const string DATE_FORMAT = "dd/MM HH:mm";
        private const string LONG_DATE_FORMAT = "dd/MM/yyyy HH:mm";

        public ReservationItineraryPdfService(IQrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        public byte[] GenerateItineraryPdf(ReservationSearchResponseDTO reservation)
        {
            var airlineName = reservation.Journey.Segments.FirstOrDefault()?.Airline ?? DEFAULT_AIRLINE_NAME;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(PAGE_MARGIN);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(DEFAULT_FONT_SIZE).FontFamily(Fonts.Arial));

                    page.Header().Element(c => ComposeHeader(c, airlineName, reservation));
                    
                    page.Content().PaddingVertical(SECTION_PADDING).Column(col => 
                    {
                        ComposePassengerSection(col, reservation);
                        ComposeJourneySection(col, reservation);
                        ComposeBoardingPasses(col, reservation);
                    });

                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("Generado por Zuli Airlines - Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private void ComposeHeader(IContainer container, string airlineName, ReservationSearchResponseDTO reservation)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(airlineName).FontSize(TITLE_FONT_SIZE).Bold().FontColor(PRIMARY_COLOR);
                    col.Item().Text(DOC_TITLE).FontSize(SUBTITLE_FONT_SIZE).SemiBold().FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(150).AlignRight().Column(col =>
                {
                    col.Item().Text(LBL_RESERVATION).FontSize(DEFAULT_FONT_SIZE).FontColor(Colors.Grey.Medium);
                    col.Item().Text(reservation.ReservationCode).FontSize(16).Bold().FontColor(PRIMARY_COLOR);
                    col.Item().Text($"{LBL_CONTACT} {reservation.ContactEmail}").FontSize(9);
                });
            });
        }

        private void ComposePassengerSection(ColumnDescriptor column, ReservationSearchResponseDTO reservation)
        {
            column.Item().PaddingBottom(10).Text(SEC_PASSENGERS).FontSize(SUBTITLE_FONT_SIZE).Bold().FontColor(Colors.Black);
            
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(h =>
                {
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Pasajero").Bold();
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Documentado").Bold();
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Mano").Bold();
                });

                foreach (var p in reservation.Passengers)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text($"{p.FirstName} {p.FirstLastName} {p.SecondLastName}");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text($"{p.CheckedBaggageQuantity} Maleta(s)");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text($"{p.CarryOnQuantity} Pieza(s)");
                }
            });
            column.Item().PaddingBottom(SECTION_PADDING);
        }

        private void ComposeJourneySection(ColumnDescriptor column, ReservationSearchResponseDTO reservation)
        {
            column.Item().PaddingBottom(10).Text(SEC_ITINERARY).FontSize(SUBTITLE_FONT_SIZE).Bold().FontColor(Colors.Black);

            var isDirect = reservation.Journey.Stops == 0;
            column.Item().Text($"Ruta: {reservation.Journey.OriginCity} ({reservation.Journey.OriginCode}) a {reservation.Journey.DestinationCity} ({reservation.Journey.DestinationCode})").SemiBold();
            column.Item().Text($"Conexiones: {(isDirect ? "Vuelo Directo" : $"{reservation.Journey.Stops} Escala(s)")}");
            
            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(cols => {
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn();
                    cols.RelativeColumn(2);
                    cols.RelativeColumn();
                });

                table.Header(h =>
                {
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Aerolínea / Vuelo").Bold();
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Aeronave").Bold();
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Ruta").Bold();
                    h.Cell().Background(HEADER_BG_COLOR).Padding(CELL_PADDING).Text("Horario").Bold();
                });

                foreach (var segment in reservation.Journey.Segments)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text($"{segment.Airline}\n{segment.FlightNumber}").SemiBold();
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text(segment.AircraftModel);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text($"{segment.OriginCode} → {segment.DestinationCode}");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(CELL_PADDING)
                        .Text($"Salida: {segment.DepartureDateTime.ToString(DATE_FORMAT)}\nLlegada: {segment.ArrivalDateTime.ToString(DATE_FORMAT)}");
                }
            });
            column.Item().PaddingBottom(SECTION_PADDING);
        }

        private void ComposeBoardingPasses(ColumnDescriptor column, ReservationSearchResponseDTO reservation)
        {
            column.Item().PaddingBottom(10).Text(SEC_BOARDING_PASSES).FontSize(SUBTITLE_FONT_SIZE).Bold().FontColor(Colors.Black);

            foreach (var segment in reservation.Journey.Segments)
            {
                foreach (var p in reservation.Passengers)
                {
                    string fullName = $"{p.FirstName} {p.FirstLastName} {p.SecondLastName}";
                    string qrData = $"Reserva: {reservation.ReservationCode} | Pasajero: {fullName} | Vuelo: {segment.FlightNumber} | Ruta: {segment.OriginCode}-{segment.DestinationCode}";
                    
                    byte[] qrImage = _qrCodeService.Generate(qrData);

                    column.Item().PaddingBottom(15).Background(Colors.White).Border(1).BorderColor(PRIMARY_COLOR).Padding(15).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(fullName).FontSize(14).Bold();
                            col.Item().Text($"Vuelo: {segment.FlightNumber}").FontSize(12).SemiBold().FontColor(PRIMARY_COLOR);
                            col.Item().Text($"{segment.OriginCode} → {segment.DestinationCode}");
                            col.Item().Text($"Salida: {segment.DepartureDateTime.ToString(LONG_DATE_FORMAT)}");
                            col.Item().PaddingTop(5).Text($"Clase: {reservation.Journey.FlightClass}").SemiBold();
                            col.Item().Text("Puerta: Por asignar").SemiBold().FontColor(Colors.Orange.Darken2);
                        });

                        row.ConstantItem(80).AlignRight().AlignMiddle().Image(qrImage);
                    });
                }
            }
        }
    }
}
