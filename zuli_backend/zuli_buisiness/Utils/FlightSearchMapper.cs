using System.Globalization;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;

namespace zuli_Business.Utils
{
    public class FlightSearchMapper : IFlightSearchMapper
    {
        private const string CULTURE_CODE = "es-ES";
        private const string TIME_FORMAT = "h:mm tt";
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string SHORT_DATE_FORMAT = "dd MMM";
        private const int TOTAL_STOPS_OFFSET = 1;

        public List<FlightSearchResponseDTO> MapToOptions(List<List<RawFlightEntity>> paths, string flightClass)
        {
            List<FlightSearchResponseDTO> result = new List<FlightSearchResponseDTO>(paths.Count);
            CultureInfo culture = new CultureInfo(CULTURE_CODE);

            foreach (List<RawFlightEntity> path in paths)
            {
                FlightSearchResponseDTO option = BuildSearchResponse(path, culture);
                result.Add(option);
            }

            return result;
        }

        private FlightSearchResponseDTO BuildSearchResponse(List<RawFlightEntity> path, CultureInfo culture)
        {
            RawFlightEntity firstFlight = path.First();
            RawFlightEntity lastFlight = path.Last();
            int totalSeconds = CalculateTotalTripDuration(path);

            FlightSearchResponseDTO option = new FlightSearchResponseDTO
            {
                PathIds = string.Join(",", path.Select(flight => flight.FlightRouteId)),
                Origin = firstFlight.Origin,
                Destination = lastFlight.Destination,
                DepartureTimeText = firstFlight.DepartureTime.ToString(TIME_FORMAT, culture).ToLower(),
                ArrivalTimeText = lastFlight.ArrivalTime.ToString(TIME_FORMAT, culture).ToLower(),
                ArrivalDateText = GetArrivalDateText(firstFlight.DepartureTime, lastFlight.ArrivalTime, culture),
                TotalDurationText = FormatDuration(totalSeconds),
                Stops = path.Count - TOTAL_STOPS_OFFSET,
                TotalTouristPrice = path.Sum(flight => flight.TouristPrice),
                TotalFirstClassPrice = path.Sum(flight => flight.FirstClassPrice),
                Segments = BuildSegments(path, culture)
            };

            return option;
        }

        private List<FlightSegmentDTO> BuildSegments(List<RawFlightEntity> path, CultureInfo culture)
        {
            List<FlightSegmentDTO> segments = new List<FlightSegmentDTO>(path.Count);
            RawFlightEntity? previousFlight = null;

            foreach (RawFlightEntity currentFlight in path)
            {
                string layoverTime = string.Empty;

                if (previousFlight != null)
                {
                    int layoverSeconds = (int)(currentFlight.DepartureTime - previousFlight.ArrivalTime).TotalSeconds;
                    layoverTime = FormatDuration(layoverSeconds);
                }

                segments.Add(new FlightSegmentDTO
                {
                    FlightId = currentFlight.FlightRouteId,
                    Origin = currentFlight.Origin,
                    Destination = currentFlight.Destination,
                    DepartureTimeText = currentFlight.DepartureTime.ToString(TIME_FORMAT, culture).ToLower(),
                    ArrivalTimeText = currentFlight.ArrivalTime.ToString(TIME_FORMAT, culture).ToLower(),
                    DurationText = FormatDuration(currentFlight.EstimatedDuration),
                    LayoverTimeText = layoverTime,
                    DepartureDateText = currentFlight.DepartureTime.ToString(DATE_FORMAT, culture)
                });

                previousFlight = currentFlight;
            }

            return segments;
        }

        private string GetArrivalDateText(DateTime departure, DateTime arrival, CultureInfo culture)
        {
            if (arrival.Date > departure.Date)
            {
                return $"Llega el {arrival.ToString(SHORT_DATE_FORMAT, culture)}";
            }
            return string.Empty;
        }

        private string FormatDuration(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)time.TotalHours}H, {time.Minutes}M";
        }

        private int CalculateTotalTripDuration(List<RawFlightEntity> path)
        {
            int totalSeconds = 0;
            RawFlightEntity? previousFlight = null;

            foreach (RawFlightEntity currentFlight in path)
            {
                totalSeconds += currentFlight.EstimatedDuration;
                if (previousFlight != null)
                {
                    totalSeconds += (int)(currentFlight.DepartureTime - previousFlight.ArrivalTime).TotalSeconds;
                }
                previousFlight = currentFlight;
            }

            return totalSeconds;
        }
    }
}