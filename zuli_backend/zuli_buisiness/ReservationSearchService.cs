using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapsterMapper;
using zuli_Business.DTO.ReservationSearch;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class ReservationSearchService : IReservationSearchService
    {
        private const string PDF_CONTENT_TYPE = "application/pdf";
        private const string PDF_FILENAME_FORMAT = "Itinerario_{0}.pdf";

        private readonly IReservationSearchRepository _repository;
        private readonly FluentValidation.IValidator<ReservationSearchRequestDTO> _validator;
        private readonly IMapper _mapper;
        private readonly TimeProvider _timeProvider;
        private readonly IReservationItineraryPdfService _pdfService;

        public ReservationSearchService(
            IReservationSearchRepository repository,
            FluentValidation.IValidator<ReservationSearchRequestDTO> validator,
            IMapper mapper,
            TimeProvider timeProvider,
            IReservationItineraryPdfService pdfService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _pdfService = pdfService;
        }

        public async Task<ReservationSearchResponseDTO> GetReservationDetailsAsync(
            ReservationSearchRequestDTO request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            validationResult.ThrowIfInvalid();

            string normalizedLastName = StringHelper.Normalize(request.LastName);
            string searchParam = $"{normalizedLastName}%";

            var (flights, passengers) = await _repository.GetReservationDataAsync(
                request.ReservationCode,
                searchParam
            );

            if (!flights.Any())
            {
                throw new ZuliNotFoundException(
                    "No se encontró ninguna reservación con los datos proporcionados."
                );
            }

            foreach (var flight in flights)
            {
                if (flight.ArrivalDateTime < flight.DepartureDateTime)
                {
                    flight.ArrivalDateTime = flight.ArrivalDateTime.AddDays(1);
                }
            }

            var firstFlight = flights.First();

            var daysRemaining =
                (
                    firstFlight.DepartureDateTime.Date -
                    _timeProvider.GetLocalNow().DateTime.Date
                ).Days;

            var response = _mapper.Map<ReservationSearchResponseDTO>(flights);

            response.ReservationCode = request.ReservationCode;
            response.DaysRemaining = daysRemaining;
            response.PassengerCount = passengers.Count;
            response.Passengers = _mapper.Map<List<ReservationSearchPassengerDTO>>(passengers);
            response.Journey.Layovers = CalculateLayovers(flights);
            response.ReservationStatusId = firstFlight.ReservationStatusId;

            return response;
        }

        private List<ReservationSearchLayoverDTO> CalculateLayovers(
            List<ReservationSearchFlightEntity> flights)
        {
            var layovers = new List<ReservationSearchLayoverDTO>();

            for (int i = 0; i < flights.Count - 1; i++)
            {
                layovers.Add(new ReservationSearchLayoverDTO
                {
                    AirportCode = flights[i].DestinationCode,
                    DurationMinutes = (int)(
                        flights[i + 1].DepartureDateTime -
                        flights[i].ArrivalDateTime
                    ).TotalMinutes
                });
            }

            return layovers;
        }

        public async Task<(byte[] FileContents, string ContentType, string FileName)> GenerateItineraryPdfAsync(ReservationSearchRequestDTO request)
        {
            var reservationDetails = await GetReservationDetailsAsync(request);
            var pdfBytes = _pdfService.GenerateItineraryPdf(reservationDetails);
            
            var fileName = string.Format(PDF_FILENAME_FORMAT, request.ReservationCode);

            return (pdfBytes, PDF_CONTENT_TYPE, fileName);
        }
    } 
}