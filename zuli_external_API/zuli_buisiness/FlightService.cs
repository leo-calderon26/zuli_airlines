using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Options;
using System.Text.Json;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Utils;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        private readonly AirlineClientDTO _airlineClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFlightPathFinder _pathFinder;
        private readonly IFlightDateGenerator _dateGenerator;
        private readonly IFlightRepository _repository;
        private readonly FlightValidator _validator;
        private readonly IMapper _mapper;
        public FlightService(IFlightPathFinder pathFinder, IFlightRepository repository, IFlightDateGenerator dateGenerator,
            IMapper mapper, IHttpClientFactory httpClientFactory, IOptions<AirlineClientDTO> airlineClientOptions)
        {
            _airlineClient = airlineClientOptions.Value;
            _httpClientFactory = httpClientFactory;

            _pathFinder = pathFinder;
            _repository = repository;
            _dateGenerator = dateGenerator;
            _validator = new FlightValidator();
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight)
        {
            _validator.ValidateRequestedFlightInfo(requestedFlight);

            List<RawFlightEntity> rawFlights = await FetchAvailableFlights(requestedFlight.earliestDeparture, requestedFlight.destination, requestedFlight.passengersQuantity);

            List<RawFlightEntity> scheduledFlights = _dateGenerator.GenerateOccurrences(rawFlights, requestedFlight.earliestDeparture, requestedFlight.latestDeparture);

            PathFinderParametersDTO criteria = new PathFinderParametersDTO
            {
                FlightPool = scheduledFlights,
                Destination = requestedFlight.destination,
                EarliestDeparture = requestedFlight.earliestDeparture,
                LatestDeparture = requestedFlight.latestDeparture
            };

            List<List<RawFlightEntity>> validPaths = _pathFinder.FindPaths(criteria);
            List<RawFlightEntity> flatFlights = validPaths.SelectMany(path => path).ToList();
            List<BookedFlightDTO> formattedOptions = _mapper.Map<List<BookedFlightDTO>>(flatFlights);

            return formattedOptions;
        }
        private async Task<List<RawFlightEntity>> FetchAvailableFlights(DateTime earliestDeparture, string destination, int passengersQuantity)
        {
            var dayOneFlights = await _repository.GetAvailableFlights(earliestDeparture, destination, passengersQuantity);

            return dayOneFlights.ToList();
        }

        public async Task<ReservationResponseDTO> ReserveFlight(ReservationRequestDTO reservationInfo)
        {
            var reservationResponse = new ReservationResponseDTO();

            var client = _httpClientFactory.CreateClient();
            UriBuilder builder = new UriBuilder(_airlineClient.url);

            var requiredPurchaseInfo = await GetReservedFlightRoute(reservationInfo);
            var summarizedFlightData = await GetReservedFlightData(reservationInfo.flightGUID);

            var ticketPurchaseRequest = _mapper.Map<TicketPurchaseRequestDTO>(requiredPurchaseInfo);
            string json = JsonSerializer.Serialize(ticketPurchaseRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(builder.Uri, content);
            var jsonString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreatePurchaseException(jsonString);
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<TicketPurchaseResponseDTO>(jsonString, options);
            if (apiResponse is not null)
            {
                reservationResponse = _mapper.Map<ReservationResponseDTO>(new ReservationResponseMappingContextDTO
                {
                    ReservationRequest = reservationInfo,
                    ReservedFlightData = summarizedFlightData,
                    TicketPurchaseResponse = apiResponse
                });
            }
            return reservationResponse;
        }

        private async Task<RequiredPurchaseInfoDTO> GetReservedFlightRoute(ReservationRequestDTO reservationInfo) 
        {
            var flightRoute = await _repository.GetRouteByFlightId(reservationInfo.flightGUID);
            if (flightRoute is null)
            {
                throw new ZuliBadRequestException($"No se encontró una ruta para el vuelo {reservationInfo.flightGUID}.");
            }
            return new RequiredPurchaseInfoDTO
            {
                ReservationInfo = reservationInfo,
                SummarizedFlightRoute = flightRoute.Adapt<SummarizedFlightRouteDTO>()
            };
        }
        private async Task<ReservedFlightDTO> GetReservedFlightData(Guid reservedFlightId)
        {
            var flightData = await _repository.GetReservedFlightData(reservedFlightId);
            if (flightData is null)
            {
                throw new ZuliBadRequestException($"No se encontraron los datos del vuelo {reservedFlightId}.");
            }
            return flightData.Adapt<ReservedFlightDTO>();
        }

        private static ZuliBadRequestException CreatePurchaseException(string jsonString)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;
                var detail = root.TryGetProperty("detail", out var detailElement)
                    ? detailElement.GetString() ?? "Error al procesar la compra del tiquete."
                    : "Error al procesar la compra del tiquete.";

                var errors = new Dictionary<string, List<string>>();
                if (root.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in errorsElement.EnumerateObject())
                    {
                        errors[property.Name] = property.Value.ValueKind == JsonValueKind.Array
                            ? property.Value.EnumerateArray().Select(error => error.GetString() ?? string.Empty).Where(error => !string.IsNullOrWhiteSpace(error)).ToList()
                            : new List<string> { property.Value.ToString() };
                    }
                }

                return new ZuliBadRequestException(detail, errors);
            }
            catch (JsonException)
            {
                return new ZuliBadRequestException(string.IsNullOrWhiteSpace(jsonString) ? "Error al procesar la compra del tiquete." : jsonString);
            }
        }
    }
}
