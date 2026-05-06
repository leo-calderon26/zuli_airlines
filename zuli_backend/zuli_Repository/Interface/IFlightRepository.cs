using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IFlightRepository
    {
        Task<int> CreateFlight(FlightEntity flight);
        Task<IEnumerable<FlightEntity>> GetAllFlights();
        Task<FlightEntity?> GetFlightById(Guid id);
        Task<(IEnumerable<FlightSearchEntity> Flights, int TotalCount)> SearchFlights(
            string origin, string destination, DateTime date, int seats, int offset, int fetch);
    }
}