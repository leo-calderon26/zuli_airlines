
using System.Globalization;
using ClosedXML.Excel;
using zuli_Business.DTO.Reports;
using zuli_Business.Interface.Reports;

namespace zuli_Business.Reports
{
    public class IncomeReportExportService : IIncomeReportExportService
    {
        private static readonly string[] HEADER_STRINGS =
        [
            "Mes",
            "Cantidad de vuelos",
            "Pasajeros Primera Clase",
            "Pasajeros Clase Turista",
            "Total Pasajeros",
            "Ingreso Tiquetes",
            "Ingreso Maletas",
            "Total Ingreso"
        ];
        private static readonly CultureInfo ES_CULTURE = new CultureInfo("es-ES");
        private const string MONEY_FORMAT = "$#,##0.00";
        private const string BACKGROUNT_COLOR = "#F3E9E7"; 
        private const string ROW_COLOR = "#DCC0BE";
        private readonly IIncomeReportService _incomeReportService;
        
        public IncomeReportExportService(IIncomeReportService incomeReportService)
        {
            _incomeReportService = incomeReportService;
        }

        public async Task<Stream> GenerateIncomeReportExcelAsync(IncomeReportRequestDTO request)
        {
            var result = await _incomeReportService.GetIncomeReportAsync(request);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Ingresos");
            
            for (int index = 0; index < HEADER_STRINGS.Length; index++ )
            {
               var cell = ws.Cell(1, index + 1);
               cell.Value = HEADER_STRINGS[index];
               cell.Style.Font.Bold = true;
               cell.Style.Fill.BackgroundColor = XLColor.FromHtml(BACKGROUNT_COLOR);
               cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
               cell.Style.Border.OutsideBorderColor = XLColor.FromHtml(ROW_COLOR);
            }

            var rows = result.Rows
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ToList();
            int row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = $"{ES_CULTURE.DateTimeFormat.MonthNames[r.Month - 1]} {r.Year}";
                ws.Cell(row, 2).Value = r.Flights;
                ws.Cell(row, 3).Value = r.FirstClass;
                ws.Cell(row, 4).Value = r.TouristClass;
                ws.Cell(row, 5).Value = r.TotalPassengers;
                
                SetMoney(ws.Cell(row, 6), r.TicketIncome);
                SetMoney(ws.Cell(row, 7), r.BaggageIncome);
                SetMoney(ws.Cell(row, 8), r.TotalIncome);
                ws.Cell(row,8).Style.Font.Bold = true;
                row++;
            }

            var summary = result.Summary;
            ws.Cell(row, 1).Value = "TOTAL";
            ws.Cell(row, 2).Value = summary.TotalFlights;
            ws.Cell(row, 3).Value = summary.TotalFirstClass;
            ws.Cell(row, 4).Value = summary.TotalTouristClass;
            ws.Cell(row, 5).Value = summary.TotalPassengers;
            SetMoney(ws.Cell(row, 6), summary.TotalTicketIncome);
            SetMoney(ws.Cell(row, 7), summary.TotalBaggageIncome);
            SetMoney(ws.Cell(row, 8), summary.TotalIncome);
            
            for(int c = 1; c <= HEADER_STRINGS.Length; c++)
            {
                ws.Cell(row, c).Style.Font.Bold = true;
                ws.Cell(row, c).Style.Fill.BackgroundColor = XLColor.FromHtml(BACKGROUNT_COLOR);
                ws.Cell(row, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(row, c).Style.Border.OutsideBorderColor = XLColor.FromHtml(ROW_COLOR);
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

