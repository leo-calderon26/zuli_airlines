using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Utils;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;
        private readonly FlightValidator _validator;
        private readonly FlightSearchValidator _searchValidator;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;

        public FlightService(
            IFlightRepository repository,
            IUserRepository userRepository,
            IServiceRepository serviceRepository)
        {
            _repository = repository;
            _validator = new FlightValidator();
            _searchValidator = new FlightSearchValidator();
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<BasicResponseDTO> CreateFlight(FlightDTO flight)
        {
            _validator.ValidateFlight(flight);
            // TOD(you); tiene que validar el compa tiene permisos
            var adminId = await _userRepository.GetUserId(flight.BusinessId);

            var newFlight = new FlightEntity
            {
                Id = Guid.NewGuid(),
                Status = flight.Status,
                FlightDate = flight.FlightDate,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                RealDepartureTime = flight.RealDepartureTime,
                RealArrivalTime = flight.RealArrivalTime,
                CheckInStartTime = flight.CheckInStartTime,
                CheckInDeadline = flight.CheckInDeadline,
                AirlineId = flight.AirlineId,
                AircraftId = flight.AircraftId,
                ItineraryId = flight.ItineraryId,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                AvailableSeats = flight.AvailableSeats,
                AdminId = adminId,
                FlightRouteId = flight.FlightRouteId,
            };

            await _repository.CreateFlight(newFlight);

            if (!string.IsNullOrWhiteSpace(flight.ServiceDescription))
            {
                var newService = new ServiceEntity
                {
                    FlightId = newFlight.Id,
                    Description = flight.ServiceDescription.Trim()
                };

                await _serviceRepository.CreateService(newService);
            }

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion del vuelo correctamente",
            };
        }

        public async Task<IEnumerable<FlightDTO>> GetAllFlights()
        {
            var flights = await _repository.GetAllFlights();
            var flightList = flights.ToList();
            var businessIds = await Task.WhenAll(
                flightList.Select(f => _userRepository.GetBusinessId(f.AdminId))
            );

            var serviceDescriptions = await Task.WhenAll(
                flightList.Select(f => _serviceRepository.GetServiceByFlightId(f.Id))
            );

            return flightList.Select((f, index) => new FlightDTO
            {
                Id = f.Id,
                Status = f.Status,
                FlightDate = f.FlightDate,
                TouristPrice = f.TouristPrice,
                FirstClassPrice = f.FirstClassPrice,
                RealDepartureTime = f.RealDepartureTime,
                RealArrivalTime = f.RealArrivalTime,
                CheckInStartTime = f.CheckInStartTime,
                CheckInDeadline = f.CheckInDeadline,
                AirlineId = f.AirlineId,
                AircraftId = f.AircraftId,
                ItineraryId = f.ItineraryId,
                Duration = f.Duration,
                CarryOnPrice = f.CarryOnPrice,
                CheckedPrice = f.CheckedPrice,
                AvailableSeats = f.AvailableSeats,
                BusinessId = businessIds[index],
                FlightRouteId = f.FlightRouteId,
                ServiceDescription = serviceDescriptions[index]?.Description,
            }).ToList();
        }

        public async Task<FlightDTO?> GetFlightById(Guid id)
        {
            var flight = await _repository.GetFlightById(id);

            if (flight == null)
                return null;
            
            var businessId = await _userRepository.GetBusinessId(flight.AdminId);
            var service = await _serviceRepository.GetServiceByFlightId(flight.Id);

            return new FlightDTO
            {
                Id = flight.Id,
                Status = flight.Status,
                FlightDate = flight.FlightDate,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                RealDepartureTime = flight.RealDepartureTime,
                RealArrivalTime = flight.RealArrivalTime,
                CheckInStartTime = flight.CheckInStartTime,
                CheckInDeadline = flight.CheckInDeadline,
                AirlineId = flight.AirlineId,
                AircraftId = flight.AircraftId,
                ItineraryId = flight.ItineraryId,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                AvailableSeats = flight.AvailableSeats,
                BusinessId = businessId,
                FlightRouteId = flight.FlightRouteId,
                ServiceDescription = service?.Description,
            };
            
        }


        public async Task<FlightPaginatedResponseDTO> Search(FlightSearchRequestDTO request)
        {
            _searchValidator.ValidateSearch(request);

            DateTime endDate;
            if (request.IsRoundTrip && request.ReturnDate.HasValue)
            {
                endDate = request.ReturnDate.Value.AddDays(1);
            }
            else
            {
                endDate = request.Date.AddDays(1);
            }

            var rawFlights = await _repository.GetAvailableFlights(request.Date, endDate, request.Seats);

            var outboundPaths = FlightPathFinder.FindPaths(
                rawFlights, request.Origin, request.Destination, request.Date, request.DirectFlightsOnly);

            var response = new FlightPaginatedResponseDTO();
            response.CurrentPage = request.Page;

            var formattedOutbound = MapToOptions(outboundPaths, request.FlightClass);
            response.TotalRecordsOutbound = formattedOutbound.Count;
            response.TotalPagesOutbound = (int)Math.Ceiling(response.TotalRecordsOutbound / (double)request.PageSize);
            response.OutboundFlights = Paginate(formattedOutbound, request.Page, request.PageSize);

            if (request.IsRoundTrip && request.ReturnDate.HasValue)
            {
                var returnPaths = FlightPathFinder.FindPaths(
                    rawFlights, request.Destination, request.Origin, request.ReturnDate.Value, request.DirectFlightsOnly);
                
                var formattedReturn = MapToOptions(returnPaths, request.FlightClass);
                response.TotalRecordsReturn = formattedReturn.Count;
                response.TotalPagesReturn = (int)Math.Ceiling(response.TotalRecordsReturn / (double)request.PageSize);
                response.ReturnFlights = Paginate(formattedReturn, request.Page, request.PageSize);
            }

            return response;
        }

        private List<FlightSearchResponseDTO> Paginate(List<FlightSearchResponseDTO> source, int page, int pageSize)
        {
            return source
                .OrderBy(x => x.TotalPrice)
                .ThenBy(x => x.Stops)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private List<FlightSearchResponseDTO> MapToOptions(List<List<RawFlightEntity>> paths, string flightClass)
        {
            var result = new List<FlightSearchResponseDTO>();
            var culture = new CultureInfo("es-ES");

            foreach (var path in paths)
            {
                var firstFlight = path.First();
                var lastFlight = path.Last();
                int totalSeconds = (int)(lastFlight.ArrivalTime - firstFlight.DepartureTime).TotalSeconds;

                string arrivalDateText = "";
                if (lastFlight.ArrivalTime.Date > firstFlight.DepartureTime.Date)
                {
                    arrivalDateText = $"Llega el {lastFlight.ArrivalTime.ToString("dd MMM", culture)}";
                }

                decimal totalPrice = 0;
                if (flightClass == "Primera Clase")
                {
                    totalPrice = path.Sum(f => f.FirstClassPrice);
                }
                else
                {
                    totalPrice = path.Sum(f => f.TouristPrice);
                }

                var option = new FlightSearchResponseDTO();
                option.PathIds = string.Join(",", path.Select(p => p.FlightId));
                option.Origin = firstFlight.Origin;
                option.Destination = lastFlight.Destination;
                option.DepartureTimeText = firstFlight.DepartureTime.ToString("h:mm tt", culture).ToLower();
                option.ArrivalTimeText = lastFlight.ArrivalTime.ToString("h:mm tt", culture).ToLower();
                option.ArrivalDateText = arrivalDateText;
                option.TotalDurationText = FormatDuration(totalSeconds);
                option.Stops = path.Count - 1;
                option.TotalPrice = totalPrice;
                option.Segments = new List<FlightSegmentDTO>();

                for (int i = 0; i < path.Count; i++)
                {
                    var flight = path[i];
                    string layoverTime = "";

                    if (i > 0)
                    {
                        var previousFlight = path[i - 1];
                        int layoverSeconds = (int)(flight.DepartureTime - previousFlight.ArrivalTime).TotalSeconds;
                        layoverTime = FormatDuration(layoverSeconds);
                    }

                    var segment = new FlightSegmentDTO();
                    segment.FlightId = flight.FlightId;
                    segment.Origin = flight.Origin;
                    segment.Destination = flight.Destination;
                    segment.DepartureTimeText = flight.DepartureTime.ToString("h:mm tt", culture).ToLower();
                    segment.ArrivalTimeText = flight.ArrivalTime.ToString("h:mm tt", culture).ToLower();
                    segment.DurationText = FormatDuration(flight.EstimatedDuration);
                    segment.LayoverTimeText = layoverTime;

                    option.Segments.Add(segment);
                }
                result.Add(option);
            }
            return result;
        }

        private string FormatDuration(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            int totalHours = (int)time.TotalHours;
            return $"{totalHours}H, {time.Minutes}M";
        }
    }
}