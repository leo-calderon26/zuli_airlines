using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class PurchaseConfirmationPdfService : IPurchaseConfirmationPdfService
    {
        private static readonly CultureInfo _cultureInfo = new("es-CR");

        public byte[] GenerateInvoicePdf(PurchaseConfirmationPageDTO confirmation)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(text => text.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text("Zuli Airlines").FontSize(20).Bold();
                            column.Item().Text("Factura de compra").FontSize(14);
                            column.Item().Text($"Código de reserva: {confirmation.ReservationCode}");
                        });

                    page.Content()
                        .PaddingVertical(25)
                        .Column(column =>
                        {
                            column.Spacing(14);

                            column.Item().Text("Datos del comprador").FontSize(13).Bold();

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                AddRow(table, "Comprador", confirmation.BuyerName);
                                AddRow(table, "Correo", confirmation.BuyerEmail);
                                AddRow(table, "Teléfono", confirmation.BuyerPhone);
                            });

                            column.Item().Text("Detalles del pago").FontSize(13).Bold();

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                AddRow(table, "Método de pago", confirmation.PaymentMethod);
                                AddRow(table, "Clase", confirmation.FlightClass);
                                AddRow(table, "Total pagado", confirmation.TotalAmount.ToString("C", _cultureInfo));
                            });

                            column.Item().Text("Este documento corresponde al comprobante de compra de la reserva indicada.");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generado por Zuli Airlines - Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                });
            }).GeneratePdf();
        }

        public byte[] GenerateConfirmationPdf(PurchaseConfirmationPageDTO confirmation)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(text => text.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text("Zuli Airlines").FontSize(20).Bold();
                            column.Item().Text("Confirmación de compra").FontSize(14);
                            column.Item().Text($"Código de reserva: {confirmation.ReservationCode}");
                        });

                    page.Content()
                        .PaddingVertical(25)
                        .Column(column =>
                        {
                            column.Spacing(16);

                            column.Item().Text("Reserva completada exitosamente.").FontSize(13).Bold();

                            column.Item().Text("Información del itinerario").FontSize(13).Bold();

                            foreach (var flight in confirmation.Flights)
                            {
                                column.Item()
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Column(flightColumn =>
                                    {
                                        flightColumn.Spacing(4);

                                        flightColumn.Item().Text($"Vuelo {flight.FlightNumber}").Bold();
                                        flightColumn.Item().Text($"Aerolínea: {flight.AirlineName}");
                                        flightColumn.Item().Text($"Origen: {flight.OriginAirportName} ({flight.OriginAirportCode})");
                                        flightColumn.Item().Text($"Destino: {flight.DestinationAirportName} ({flight.DestinationAirportCode})");
                                        flightColumn.Item().Text($"Salida: {flight.DepartureDateTime:dd/MM/yyyy HH:mm}");
                                        flightColumn.Item().Text($"Llegada: {flight.ArrivalDateTime:dd/MM/yyyy HH:mm}");
                                    });
                            }

                            column.Item().Text("Información de pasajeros").FontSize(13).Bold();

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    AddHeaderCell(header, "Nombre");
                                    AddHeaderCell(header, "Nacimiento");
                                    AddHeaderCell(header, "Género");
                                    AddHeaderCell(header, "Pasaporte");
                                    AddHeaderCell(header, "Equipaje");
                                });

                                foreach (var passenger in confirmation.Passengers)
                                {
                                    AddCell(table, passenger.FullName);
                                    AddCell(table, passenger.BirthDate);
                                    AddCell(table, passenger.Gender);
                                    AddCell(table, passenger.PassportCountry);
                                    AddCell(table, $"Maletas: {passenger.CheckedBaggageQuantity} / Carry on: {passenger.CarryOnQuantity}");
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generado por Zuli Airlines - Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                });
            }).GeneratePdf();
        }

        private static void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6)
                .Text(label)
                .Bold();

            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6)
                .Text(value);
        }

        private static void AddHeaderCell(TableCellDescriptor header, string text)
        {
            header.Cell()
                .Background(Colors.Grey.Lighten3)
                .Padding(6)
                .Text(text)
                .Bold();
        }

        private static void AddCell(TableDescriptor table, string text)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6)
                .Text(text);
        }
    }
}