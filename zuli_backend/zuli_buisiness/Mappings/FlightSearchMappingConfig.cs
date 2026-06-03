using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class FlightSearchMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            var culture = new CultureInfo("es-ES");

            config.NewConfig<List<RawFlightEntity>, FlightSearchResponseDTO>()
                .MapWith(path => new FlightSearchResponseDTO
                {
                    FlightId = path.First().FlightId,
                    PathIds = string.Join(",", path.Select(f => f.FlightRouteId)),
                    Origin = path.First().Origin,
                    Destination = path.Last().Destination,
                    DepartureTimeText = path.First().DepartureTime.ToString("h:mm tt", culture).ToLower(),
                    ArrivalTimeText = path.Last().ArrivalTime.ToString("h:mm tt", culture).ToLower(),
                    ArrivalDateText = GetArrivalDateText(path.First().DepartureTime, path.Last().ArrivalTime, culture),
                    TotalDurationText = FormatDuration(
                        path.Sum(f => f.EstimatedDuration) + 
                        path.Skip(1).Select((f, i) => (int)(f.DepartureTime - path[i].ArrivalTime).TotalSeconds).Sum()
                    ),
                    
                    Stops = path.Count - 1,
                    TotalTouristPrice = path.Sum(f => f.TouristPrice),
                    TotalFirstClassPrice = path.Sum(f => f.FirstClassPrice),
                    CarryOnPrice = path.First().CarryOnPrice,
                    CheckedPrice = path.First().CheckedPrice,
                    MaxWeightPerBag = path.First().MaxWeightPerBag,
                    CheckedBagMultiplier = path.First().CheckedBagMultiplier,
                    
                    Segments = path.Select((current, index) => new FlightSegmentDTO
                    {
                        FlightId = current.FlightRouteId,
                        Origin = current.Origin,
                        Destination = current.Destination,
                        DepartureTimeText = current.DepartureTime.ToString("h:mm tt", culture).ToLower(),
                        ArrivalTimeText = current.ArrivalTime.ToString("h:mm tt", culture).ToLower(),
                        DurationText = FormatDuration(current.EstimatedDuration),
                        LayoverTimeText = GetLayoverTimeText(index, current, path),
                        DepartureDateText = current.DepartureTime.ToString("yyyy-MM-dd", culture),
                        CheckedPrice = current.CheckedPrice,
                        CarryOnPrice = current.CarryOnPrice,
                        CheckedBagMultiplier = current.CheckedBagMultiplier
                    }).ToList()
                });
        }


        private static string GetArrivalDateText(DateTime departure, DateTime arrival, CultureInfo culture)
        {
            if (arrival.Date > departure.Date)
            {
                return $"Llega el {arrival.ToString("dd MMM", culture)}";
            }
            
            return string.Empty;
        }

        private static string GetLayoverTimeText(int index, RawFlightEntity currentFlight, List<RawFlightEntity> path)
        {
            if (index == 0)
            {
                return string.Empty;
            }

            int layoverSeconds = (int)(currentFlight.DepartureTime - path[index - 1].ArrivalTime).TotalSeconds;
            return FormatDuration(layoverSeconds);
        }

        private static string FormatDuration(int totalSeconds)
        {
            var time = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)time.TotalHours}H, {time.Minutes}M";
        }
    }
}