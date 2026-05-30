using System;
using System.Collections.Generic;
using System.Linq;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;

namespace zuli_Business.Utils
{
    // Adapted from: https://www.geeksforgeeks.org/dsa/find-paths-given-source-destination/
    public class FlightPathFinder : IFlightPathFinder
    {
        public List<List<RawFlightEntity>> FindPaths(PathFinderParametersDTO parameters)
        {
            List<RawFlightEntity> pool = parameters.FlightPool.ToList();

            return GetDirectFlights(pool, parameters.Destination, parameters.EarliestDeparture, parameters.LatestDeparture);
        }

        private List<List<RawFlightEntity>> GetDirectFlights(
            List<RawFlightEntity> pool,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture)
        {
            return pool
                .Where(flight => flight.ArrivalAiportCode == destination &&
                                 flight.DepartureTime.Date >= earliestDeparture.Date &&
                                 flight.ArrivalTime.Date <= latestDeparture.Date)
                .Select(flight => new List<RawFlightEntity> { flight })
                .ToList();
        }
    }
}