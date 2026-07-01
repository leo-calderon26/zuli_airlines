using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IFlightRouteService
    {
        Task<BasicResponseDTO> CreateFlightRouter(FlightRouteDTO flightRouter);
        Task<FlightRoutePaginatedResponseDTO> GetFlightRoutesPaginated(int pageNumber, int pageSize);
        Task<BasicResponseDTO> DeleteFlightRoute(int flightRouteId);
    }
}
