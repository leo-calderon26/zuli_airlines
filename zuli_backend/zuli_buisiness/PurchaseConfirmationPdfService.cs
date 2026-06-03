using System;
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
        private static readonly CultureInfo _cultureInfo = new("en-US");

        public byte[] GenerateInvoicePdf(PurchaseConfirmationPageDTO confirmation)
        {
            return GenerateBasePdf(
                title: "Factura de compra",
                reservationCode: confirmation.ReservationCode,
                contentAction: column => BuildInvoiceContent(column, confirmation)
            );
        }

        public byte[] GenerateConfirmationPdf(PurchaseConfirmationPageDTO confirmation)
        {
            return GenerateBasePdf(
                title: "Confirmación de compra",
                reservationCode: confirmation.ReservationCode,
                contentAction: column => BuildConfirmationContent(column, confirmation)
            );
        }

        private byte[] GenerateBasePdf(string title, string reservationCode, Action<ColumnDescriptor> contentAction)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(text => text.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Zuli Airlines").FontSize(20).Bold();
                        column.Item().Text(title).FontSize(14);
                        column.Item().Text($"Código de reserva: {reservationCode}");
                    });

                    page.Content()
                        .PaddingVertical(25)
                        .Column(contentAction);

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generado por Zuli Airlines - Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private void BuildInvoiceContent(ColumnDescriptor column, PurchaseConfirmationPageDTO confirmation)
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

            BuildBreakdownContent(column, confirmation);

            column.Item().Text("Este documento corresponde al comprobante de compra de la reserva indicada.");
        }

        private void BuildConfirmationContent(ColumnDescriptor column, PurchaseConfirmationPageDTO confirmation)
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

            BuildBreakdownContent(column, confirmation);
        }

        private void BuildBreakdownContent(ColumnDescriptor column, PurchaseConfirmationPageDTO confirmation)
        {
            if (confirmation.Breakdown == null || confirmation.Breakdown.Flights.Count == 0)
                return;

            column.Item().Text("Desglose de costos").FontSize(13).Bold();

            foreach (var flight in confirmation.Breakdown.Flights)
            {
                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(10)
                    .Column(flightColumn =>
                    {
                        flightColumn.Spacing(6);
                        flightColumn.Item().Text($"Vuelo {flight.FlightNumber} — {flight.OriginAirportCode} → {flight.DestinationAirportCode}").FontSize(11).Bold();

                        foreach (var passenger in flight.Passengers)
                        {
                            flightColumn.Item().Text($"Pasajero: {passenger.FullName}").FontSize(10).Bold();

                            flightColumn.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    AddHeaderCell(header, "Concepto");
                                    AddHeaderCell(header, "Cant.");
                                    AddHeaderCell(header, "Precio");
                                });

                                AddCell(table, "Boleto de avión");
                                AddCell(table, "1");
                                AddCell(table, passenger.TicketPrice.ToString("C", _cultureInfo));

                                foreach (var bag in passenger.CheckedBags)
                                {
                                    AddCell(table, $"  Maleta documentada #{bag.BagNumber}");
                                    AddCell(table, "1");
                                    AddCell(table, bag.Price.ToString("C", _cultureInfo));
                                }

                                if (passenger.CarryOnQuantity > 0)
                                {
                                    AddCell(table, "  Equipaje de mano");
                                    AddCell(table, passenger.CarryOnQuantity.ToString());
                                    AddCell(table, passenger.CarryOnTotal.ToString("C", _cultureInfo));
                                }

                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Subtotal pasajero").Bold();
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("");
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(passenger.PassengerTotal.ToString("C", _cultureInfo)).Bold();
                            });
                        }

                        flightColumn.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Cell().Background(Colors.Grey.Lighten1).Padding(6).Text($"Total vuelo {flight.FlightNumber}").Bold().FontColor(Colors.White);
                            table.Cell().Background(Colors.Grey.Lighten1).Padding(6).Text("");
                            table.Cell().Background(Colors.Grey.Lighten1).Padding(6).Text(flight.FlightTotal.ToString("C", _cultureInfo)).Bold().FontColor(Colors.White);
                        });
                    });
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Background(Colors.Grey.Medium).Padding(8).Text("TOTAL GENERAL").Bold().FontSize(11).FontColor(Colors.White);
                table.Cell().Background(Colors.Grey.Medium).Padding(8).Text("");
                table.Cell().Background(Colors.Grey.Medium).Padding(8).Text(confirmation.TotalAmount.ToString("C", _cultureInfo)).Bold().FontSize(11).FontColor(Colors.White);
            });
        }

        private static void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(label).Bold();
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(value);
        }

        private static void AddHeaderCell(TableCellDescriptor header, string text)
        {
            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(text).Bold();
        }

        private static void AddCell(TableDescriptor table, string text)
        {
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(text);
        }
    }
}