using System.Globalization;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;

namespace zuli_Business.Utils
{
    public class FlightSearchMapper : IFlightSearchMapper
    {
        public List<FlightSearchResponseDTO> MapToOptions(List<List<RawFlightEntity>> paths, string flightClass)
        {
            var result = new List<FlightSearchResponseDTO>(paths.Count);
            var culture = new CultureInfo("es-ES");

            foreach (var path in paths)
            {
                var firstFlight = path.First();
                var lastFlight = path.Last();
                int totalSeconds = (int)(lastFlight.ArrivalTime - firstFlight.DepartureTime).TotalSeconds;

                var option = new FlightSearchResponseDTO
                {
                    PathIds = string.Join(",", path.Select(p => p.FlightRouteId)),
                    Origin = firstFlight.Origin,
                    Destination = lastFlight.Destination,
                    DepartureTimeText = firstFlight.DepartureTime.ToString("h:mm tt", culture).ToLower(),
                    ArrivalTimeText = lastFlight.ArrivalTime.ToString("h:mm tt", culture).ToLower(),
                    ArrivalDateText = lastFlight.ArrivalTime.Date > firstFlight.DepartureTime.Date
                        ? $"Llega el {lastFlight.ArrivalTime.ToString("dd MMM", culture)}" : "",
                    TotalDurationText = FormatDuration(totalSeconds),
                    Stops = path.Count - 1,
                    TotalPrice = CalculateTotalPrice(path, flightClass),
                    Segments = new List<FlightSegmentDTO>(path.Count)
                };

                for (int i = 0; i < path.Count; i++)
                {
                    var flight = path[i];
                    string layoverTime = i > 0
                        ? FormatDuration((int)(flight.DepartureTime - path[i - 1].ArrivalTime).TotalSeconds) : "";

                    option.Segments.Add(new FlightSegmentDTO
                    {
                        FlightId = flight.FlightRouteId,
                        Origin = flight.Origin,
                        Destination = flight.Destination,
                        DepartureTimeText = flight.DepartureTime.ToString("h:mm tt", culture).ToLower(),
                        ArrivalTimeText = flight.ArrivalTime.ToString("h:mm tt", culture).ToLower(),
                        DurationText = FormatDuration(flight.EstimatedDuration),
                        LayoverTimeText = layoverTime
                    });
                }
                result.Add(option);
            }
            return result;
        }

        private decimal CalculateTotalPrice(List<RawFlightEntity> path, string flightClass)
        {
            return path.Sum(f => flightClass == "Primera Clase" ? f.FirstClassPrice : f.TouristPrice);
        }

        private string FormatDuration(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)time.TotalHours}H, {time.Minutes}M";
        }
    }
}