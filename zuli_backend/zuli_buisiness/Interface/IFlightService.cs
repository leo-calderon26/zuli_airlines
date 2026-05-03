using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;

namespace zuli_Buisiness.Interface
{
    public interface IFlightService
    {
        Task<BasicResponseDTO> CreateFlight(FlightDTO flight);
        Task<IEnumerable<FlightDTO>> GetAllFlights();
        Task<FlightDTO?> GetFlightById(Guid id);
    }
}
