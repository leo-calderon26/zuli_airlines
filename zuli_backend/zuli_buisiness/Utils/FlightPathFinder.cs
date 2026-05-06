// ARCHIVO: zuli_Business/Utils/FlightPathFinder.cs
using System;
using System.Collections.Generic;
using System.Linq;
using zuli_Data.Entities;

namespace zuli_Business.Utils
{
    // Adapted from: https://www.geeksforgeeks.org/dsa/find-paths-given-source-destination/
    public static class FlightPathFinder
    {
        public static List<List<RawFlightEntity>> FindPaths(
            IEnumerable<RawFlightEntity> allFlights,
            string origin,
            string destination,
            DateTime targetDate,
            bool directFlightsOnly)
        {
            var allPaths = new List<List<RawFlightEntity>>();
            var currentPath = new List<RawFlightEntity>();


            var flightPool = allFlights.ToList();

            if (directFlightsOnly)
            {
                return flightPool
                    .Where(f =>
                        f.Origin == origin &&
                        f.Destination == destination &&
                        f.DepartureTime.Date == targetDate.Date)
                    .Select(f => new List<RawFlightEntity> { f })
                    .ToList();
            }

            void DFS(string currentAirport, DateTime currentArrivalTime)
            {
                if (currentAirport == destination)
                {
                    allPaths.Add(new List<RawFlightEntity>(currentPath));
                    return;
                }

                if (currentPath.Count >= 4)
                {
                    return;
                }

                var adyacentes = flightPool.Where(f => f.Origin == currentAirport);

                foreach (var nextFlight in adyacentes)
                {
                    if (currentPath.Any(p => p.Origin == nextFlight.Destination))
                    {
                        continue;
                    }

                    if (currentPath.Count > 0)
                    {
                        if (nextFlight.DepartureTime < currentArrivalTime.AddHours(1))
                        {
                            continue;
                        }

                        if (nextFlight.DepartureTime > currentArrivalTime.AddHours(12))
                        {
                            continue;
                        }
                    }

                    currentPath.Add(nextFlight);

                    DFS(nextFlight.Destination, nextFlight.ArrivalTime);

                    currentPath.RemoveAt(currentPath.Count - 1);
                }
            }

            var startingFlights = flightPool.Where(f =>
                f.Origin == origin &&
                f.DepartureTime.Date == targetDate.Date);

            foreach (var firstFlight in startingFlights)
            {
                currentPath.Add(firstFlight);
                DFS(firstFlight.Destination, firstFlight.ArrivalTime);
                currentPath.RemoveAt(currentPath.Count - 1);
            }

            return allPaths;
        }
    }
}