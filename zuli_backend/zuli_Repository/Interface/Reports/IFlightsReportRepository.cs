using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data.Entities.Reports;
using zuli_Data.Entities.Filters;

namespace zuli_Repository.Interface
{
    public interface IFlightsReportRepository
    {
        Task<IEnumerable<FlightsReportEntity>> GetFlightsReportAsync(FlightsReportFilterEntity filters);
    }
}