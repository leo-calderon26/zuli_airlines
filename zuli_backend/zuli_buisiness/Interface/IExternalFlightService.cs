using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO.External;

namespace zuli_Business.Interface
{
    public interface IExternalFlightService
    {
        Task<IEnumerable<RetrievedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight);
    }
}
