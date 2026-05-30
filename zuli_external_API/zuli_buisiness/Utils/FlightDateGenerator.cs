using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using zuli_Business.Interface;
using zuli_Data.Entities;

namespace zuli_Business.Utils
{
    public class FlightDateGenerator : IFlightDateGenerator
    {
        public List<RawFlightEntity> GenerateOccurrences(
        IEnumerable<RawFlightEntity> routes,
        DateTime earliestDeparture,
        DateTime latestDeparture)
        {
            List<RawFlightEntity> occurrences = new();

            foreach (var route in routes)
            {
                DateTime current = earliestDeparture.Date;

                while (current <= latestDeparture.Date)
                {
                    int currentMask = current.ToDayOfWeekMask();

                    bool operatesToday =
                        (route.Frequency & currentMask) > 0;

                    if (operatesToday)
                    {
                        DateTime departureTime =
                            current.Date
                            + route.ScheduledDepartureTime;

                        if (
                            departureTime >= earliestDeparture &&
                            departureTime <= latestDeparture
                        )
                        {
                            DateTime arrivalTime =
                                current.Date
                                + route.ScheduledArrivalTime;

                            // Manejo de vuelos que cruzan medianoche
                            if (route.ScheduledArrivalTime <
                                route.ScheduledDepartureTime)
                            {
                                arrivalTime = arrivalTime.AddDays(1);
                            }

                            occurrences.Add(new RawFlightEntity
                            {
                                FlightGUID = CreateFlightGuid(route.FlightRouteId, departureTime),
                                FlightRouteId = route.FlightRouteId,

                                DepartureTime = departureTime,
                                ArrivalTime = arrivalTime,

                                EstimatedDuration = route.EstimatedDuration,

                                DepartureAiportCode = route.DepartureAiportCode,
                                DepartureAiportName = route.DepartureAiportName,
                                DepartureCityName = route.DepartureCityName,

                                ArrivalAiportCode = route.ArrivalAiportCode,
                                ArrivalAirportName = route.ArrivalAirportName,
                                ArrivalAirportCity = route.ArrivalAirportCity,

                                TouristPrice = route.TouristPrice,
                                FirstClassPrice = route.FirstClassPrice,
                                CarryOnPrice = route.CarryOnPrice,
                                CheckedPrice = route.CheckedPrice,
                            });
                        }
                    }

                    current = current.AddDays(1);
                }
            }

            return occurrences;
        }

        private static Guid CreateFlightGuid(int flightRouteId, DateTime departureTime)
        {
            string input = $"{flightRouteId}|{departureTime:O}";
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
