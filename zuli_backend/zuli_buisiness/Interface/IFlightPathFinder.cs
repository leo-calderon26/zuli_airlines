using System.Collections.Generic;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Interface
{
    public interface IFlightPathFinder
    {
        List<List<RawFlightEntity>> FindPaths(PathFinderParametersDTO parameters);
    }
}