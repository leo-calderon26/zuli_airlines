using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;

namespace zuli_Business.Interface
{
    public interface IFlightService
    {
        Task<IEnumerable<BookedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight);
    }
}
