using System.Collections.Generic;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Interface
{
    public interface IFlightDateGenerator
    {
        List<RawFlightEntity> GenerateOccurrences(
        IEnumerable<RawFlightEntity> routes,
        DateTime earliestDeparture,
        DateTime latestDeparture);
    }
}