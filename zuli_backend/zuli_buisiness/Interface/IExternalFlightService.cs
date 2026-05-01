using System;
using System.Collections.Generic;
using System.Text;
<<<<<<< HEAD
using zuli_Business.DTO.External;

namespace zuli_Business.Interface
=======
using zuli_Buisiness.DTO.External;

namespace zuli_Buisiness.Interface
>>>>>>> 2786cd1 (Se borró el script viejo para la creación de la tabla flight. Así mismo se corrigen los nombres de las clases.)
{
    public interface IExternalFlightService
    {
        Task<IEnumerable<RetrievedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight);
    }
}
