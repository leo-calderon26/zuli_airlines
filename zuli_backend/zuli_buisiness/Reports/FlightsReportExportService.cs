using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using zuli_Business.DTO.Filters;
using zuli_Business.Interface;
using zuli_Business.Interface.Reports;

namespace zuli_Business.Reports
{
    public class FlightsReportExportService : IFlightsReportExportService
    {
        private static readonly string[] HEADER_STRINGS =
        [
            "Fecha",
            "Origen",
            "Destino",
            "Número Vuelo",
            "Aerolínea",
            "Pasajeros Primera",
            "Pasajeros Económica",
            "Venta Pasajeros",
            "Venta Equipaje",
            "Total Venta"
        ];

        private const string MONEY_FORMAT = "$#,##0.00";
        private const string BACKGROUND_COLOR = "#F3E9E7";
        private const string ROW_COLOR = "#DCC0BE";

        private readonly IFlightsReportService _flightsReportService;

        public FlightsReportExportService(IFlightsReportService flightsReportService)
        {
            _flightsReportService = flightsReportService;
        }

        public async Task<Stream> GenerateFlightsReportExcelAsync(FlightsReportFilterDTO request)
        {
            var result = await _flightsReportService.GetFlightsReportAsync(request);
            var rows = result.ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte de Vuelos");

            for (int index = 0; index < HEADER_STRINGS.Length; index++)
            {
                var cell = ws.Cell(1, index + 1);
                cell.Value = HEADER_STRINGS[index];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml(BACKGROUND_COLOR);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml(ROW_COLOR);
            }

            int row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.Fecha.ToString("dd/MM/yyyy");
                ws.Cell(row, 2).Value = r.Origen;
                ws.Cell(row, 3).Value = r.Destino;
                ws.Cell(row, 4).Value = r.NumeroVuelo;
                ws.Cell(row, 5).Value = r.Aerolinea;
                ws.Cell(row, 6).Value = r.PasajerosPrimera;
                ws.Cell(row, 7).Value = r.PasajerosEconomica;

                SetMoney(ws.Cell(row, 8), r.VentaPasajeros);
                SetMoney(ws.Cell(row, 9), r.VentaEquipaje);
                SetMoney(ws.Cell(row, 10), r.TotalVenta);
                ws.Cell(row, 10).Style.Font.Bold = true;

                row++;
            }

            if (rows.Count > 0)
            {
                ws.Cell(row, 1).Value = string.Empty;
                ws.Cell(row, 2).Value = string.Empty;
                ws.Cell(row, 3).Value = string.Empty;
                ws.Cell(row, 4).Value = string.Empty;


                ws.Cell(row, 5).Value = "Total pasajeros";
                ws.Cell(row, 5).Style.Font.Bold = true;
                ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(row, 6).FormulaA1 = $"=SUM(F2:F{row - 1})";
                ws.Cell(row, 6).Style.Font.Bold = true;

                ws.Cell(row, 7).FormulaA1 = $"=SUM(G2:G{row - 1})";
                ws.Cell(row, 7).Style.Font.Bold = true;

                ws.Cell(row, 8).FormulaA1 = $"=SUM(H2:H{row - 1})";
                ws.Cell(row, 8).Style.Font.Bold = true;
                ws.Cell(row, 8).Style.NumberFormat.Format = "\"Total \"$#,##0.00";

                ws.Cell(row, 9).FormulaA1 = $"=SUM(I2:I{row - 1})";
                ws.Cell(row, 9).Style.Font.Bold = true;
                ws.Cell(row, 9).Style.NumberFormat.Format = "\"Total \"$#,##0.00";

                ws.Cell(row, 10).FormulaA1 = $"=SUM(J2:J{row - 1})";
                ws.Cell(row, 10).Style.Font.Bold = true;
                ws.Cell(row, 10).Style.NumberFormat.Format = "\"Total \"$#,##0.00";
            }


            ws.Columns().AdjustToContents();

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            return ms;
        }

        private static void SetMoney(IXLCell cell, decimal value)
        {
            cell.Value = value;
            cell.Style.NumberFormat.Format = MONEY_FORMAT;
        }
    }
}