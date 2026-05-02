using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO.External;

namespace zuli_Buisiness.Interface
{
    public interface IExternalFlightService
    {
        Task<IEnumerable<RetrievedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight);
    }
}
