using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IFlightRepository
    {
        Task<int> CreateFlight(FlightEntity flight);
        Task<IEnumerable<FlightEntity>> GetAllFlights();
        Task<FlightEntity> GetFlightById(Guid id);
    }
}