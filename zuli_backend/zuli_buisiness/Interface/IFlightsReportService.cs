using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IFlightsReportService
    {
        Task<IEnumerable<FlightsReportDTO>> GetFlightsReportAsync();
    }
}