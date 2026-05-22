using zuli_Data.Entities;

namespace zuli_Business.Interface
{
    public interface IFlightPathFinder
    {
        List<List<RawFlightEntity>> FindPaths(IEnumerable<RawFlightEntity> allFlights, string origin, string destination, DateTime targetDate, bool directFlightsOnly);
    }
}