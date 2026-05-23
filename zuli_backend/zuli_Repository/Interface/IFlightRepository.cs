using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IFlightRepository
    {
        Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime targetDate, int seats, int targetDayMask);
        Task<int> EnsureFlightsExist(DateTime targetDate, int seats, int targetDayMask, DateTime currentTime);
        Task<int> CreateFlight(FlightEntity flight);
        Task<IEnumerable<FlightEntity>> GetAllFlights();
        Task<FlightEntity?> GetFlightById(Guid id);
    }
}