using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO.External;

namespace zuli_Buisiness.Interface
{
    public interface IFlightService
    {
        Task<IEnumerable<RetrievedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight);
    }
}
