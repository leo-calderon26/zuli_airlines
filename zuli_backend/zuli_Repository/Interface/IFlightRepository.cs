using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IFlightRepository
    {
        Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime targetDate, int seats, int targetDayMask);
        Task<Guid> CreateFlight(int flightRouteId, string DepartureDate);
        Task<Guid> GetFlightByRoute(int flightRouteId, string DepartureDate);
        Task<IEnumerable<FlightEntity>> GetAllFlights();
        Task<FlightEntity?> GetFlightById(Guid id);
        Task<int> CheckAvailability(int flightRouteId, DateTime targetDate, int seats);
    }
}