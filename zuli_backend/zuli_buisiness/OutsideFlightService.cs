using MapsterMapper;
using Microsoft.Extensions.Options;
using System.Text.Json;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Utils;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class OutsideFlightService : IOutsideFlightService
    {
        private readonly List<ExternalAirlinesDTO> _externalAirlines;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IOutsideFlightRepository _repository;
        private readonly FluentValidation.IValidator<OutsideFlightRequestDTO> _requestValidator;
        private readonly FluentValidation.IValidator<OutsideFlightDTO> _responseValidator;
        private readonly IMapper _mapper;

        public OutsideFlightService(
            IOutsideFlightRepository repository,
            FluentValidation.IValidator<OutsideFlightRequestDTO> requestValidator,
            FluentValidation.IValidator<OutsideFlightDTO> responseValidator,
            IMapper mapper, IHttpClientFactory httpClientFactory,
            IOptions<List<ExternalAirlinesDTO>> externalAirlinesOptions)
        {
            _externalAirlines = externalAirlinesOptions.Value ?? new List<ExternalAirlinesDTO>();

            _httpClientFactory = httpClientFactory;

            _repository = repository;
            _requestValidator = requestValidator;
            _responseValidator = responseValidator;
            _mapper = mapper;
        }

        public async Task FindOutsideFlights(OutsideFlightRequestDTO outsideFlight) {
            var validationResult = await _requestValidator.ValidateAsync(outsideFlight);
            validationResult.ThrowIfInvalid();
            var outsideFlights = new List<OutsideFlightDTO>();
            foreach (var externalAirline in _externalAirlines) 
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    UriBuilder builder = new UriBuilder(externalAirline.url);
                    builder.Query = $"destination={outsideFlight.Destination}&earliestdeparture={outsideFlight.EarliestDeparture.ToString("yyyy-MM-ddTHH:mm")}&latestdeparture={outsideFlight.LatestDeparture.ToString("yyyy-MM-ddTHH:mm")}&quantityOfPassengers={outsideFlight.QuantityOfPassengers}&ApiKey={externalAirline.token}";
                    var response = await client.GetAsync(builder.Uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var apiResponse = JsonSerializer.Deserialize<OutsideFlightResponseDTO>(jsonString, options);

                        if (apiResponse?.OutsideFlights != null)
                        {
                            foreach (var flight in apiResponse.OutsideFlights)
                            {
                                flight.AirlineId = externalAirline.airlineId;
                                flight.Frequency = outsideFlight.EarliestDeparture.ToDayOfWeekMask();
                                TimeSpan duracion = TimeSpan.Parse(flight.Duration);
                                flight.DurationOnSeconds = (int)duracion.TotalSeconds;
                            }

                            outsideFlights.AddRange(apiResponse.OutsideFlights);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al consultar {externalAirline.name}: {ex.Message}");
                }
            }
            await CreateOutsideFlights(outsideFlights);
        }

        public async Task UpdateOutsideFlightsStatus() {
            await _repository.UpdateOutsideFlightsStatus();
        }

        private async Task CreateOutsideFlights(List<OutsideFlightDTO> outsideFlights)
        {
            foreach (var outsideFlight in outsideFlights) {
                var validationResult = await _responseValidator.ValidateAsync(outsideFlight);
                validationResult.ThrowIfInvalid();
            }
            var outsideFlightBulk = _mapper.Map<List<OutsideFlightEntity>>(outsideFlights);
            await _repository.CreateOutsideFlightBulk(outsideFlightBulk);
        }
    }
}