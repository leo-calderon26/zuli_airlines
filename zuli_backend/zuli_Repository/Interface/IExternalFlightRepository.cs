using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities.External;

namespace zuli_Repository.Interface
{
    public interface IExternalFlightRepository
    {
        Task<IEnumerable<RetrievedFlightEntity>> RetrieveAvailableFlights(RequestedFlightEntity requestedFlight);
    }
}
