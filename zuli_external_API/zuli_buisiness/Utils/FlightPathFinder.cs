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
        private const int MIN_LAYOVER_HOURS = 1;
        private const int MAX_LAYOVER_HOURS = 12;
        private const int EXTRA_LAYOVER_OFFSET = 1;

        public List<List<RawFlightEntity>> FindPaths(PathFinderParametersDTO parameters)
        {
            List<RawFlightEntity> pool = parameters.FlightPool.ToList();

            if (parameters.DirectFlightsOnly)
            {
                return GetDirectFlights(pool, parameters.Origin, parameters.Destination, parameters.EarliestDeparture, parameters.LatestDeparture);
            }

            return GetConnectingFlights(parameters, pool);
        }

        private List<List<RawFlightEntity>> GetDirectFlights(
            List<RawFlightEntity> pool, 
            string origin, 
            string destination, 
            DateTime earliestDeparture,
            DateTime latestDeparture)
        {
            return pool
                .Where(flight => flight.DepartureAiportCode == origin && 
                                 flight.ArrivalAiportCode == destination && 
                                 flight.DepartureTime.Date >= earliestDeparture.Date &&
                                 flight.ArrivalTime.Date <= latestDeparture.Date)
                .Select(flight => new List<RawFlightEntity> { flight })
                .ToList();
        }

        private List<List<RawFlightEntity>> GetConnectingFlights(PathFinderParametersDTO parameters, List<RawFlightEntity> pool)
        {
            List<List<RawFlightEntity>> allPaths = new List<List<RawFlightEntity>>();
            List<RawFlightEntity> currentPath = new List<RawFlightEntity>();

            IEnumerable<RawFlightEntity> startingFlights = pool.Where(flight =>
                flight.DepartureAiportCode == parameters.Origin &&
                flight.DepartureTime.Date >= parameters.EarliestDeparture.Date);

            foreach (RawFlightEntity firstFlight in startingFlights)
            {
                currentPath.Add(firstFlight);
                ExploreAdjacentFlights(firstFlight.ArrivalAiportCode, firstFlight.ArrivalTime, pool, currentPath, allPaths, parameters);
                currentPath.RemoveAt(currentPath.Count - EXTRA_LAYOVER_OFFSET);
            }

            return allPaths;
        }

        private void ExploreAdjacentFlights(
            string currentAirport, 
            DateTime currentArrivalTime, 
            List<RawFlightEntity> pool, 
            List<RawFlightEntity> currentPath, 
            List<List<RawFlightEntity>> allPaths, 
            PathFinderParametersDTO parameters)
        {
            if (currentAirport == parameters.Destination)
            {
                allPaths.Add(new List<RawFlightEntity>(currentPath));
                return;
            }

            if (currentPath.Count >= parameters.MaxLayovers + EXTRA_LAYOVER_OFFSET)
            {
                return;
            }

            IEnumerable<RawFlightEntity> adjacentFlights = pool.Where(flight => flight.DepartureAiportCode == currentAirport);

            foreach (RawFlightEntity nextFlight in adjacentFlights)
            {
                if (currentPath.Any(path => path.DepartureAiportCode == nextFlight.ArrivalAiportCode))
                {
                    continue;
                }

                if (!IsValidLayover(currentArrivalTime, nextFlight.DepartureTime))
                {
                    continue;
                }

                currentPath.Add(nextFlight);
                ExploreAdjacentFlights(nextFlight.ArrivalAiportCode, nextFlight.ArrivalTime, pool, currentPath, allPaths, parameters);
                currentPath.RemoveAt(currentPath.Count - EXTRA_LAYOVER_OFFSET);
            }
        }

        private bool IsValidLayover(DateTime arrivalTime, DateTime nextDepartureTime)
        {
            DateTime minDeparture = arrivalTime.AddHours(MIN_LAYOVER_HOURS);
            DateTime maxDeparture = arrivalTime.AddHours(MAX_LAYOVER_HOURS);

            return nextDepartureTime >= minDeparture && nextDepartureTime <= maxDeparture;
        }
    }
}