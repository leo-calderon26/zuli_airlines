using System.IO;
using System.Threading.Tasks;
using zuli_Business.DTO.Reports;
using zuli_Business.DTO.Filters;

namespace zuli_Business.Interface.Reports
{
    public interface IFlightsReportExportService
    {
        Task<Stream> GenerateFlightsReportExcelAsync(FlightsReportFilterDTO request);
    }
}
