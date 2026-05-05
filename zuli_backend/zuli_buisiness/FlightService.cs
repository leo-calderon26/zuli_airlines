using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
using zuli_Buisiness.Validation;
using System.Globalization;
using System.Linq;

namespace zuli_Buisiness
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;
        private readonly FlightSearchValidator _validator;

        public FlightService(IFlightRepository repository)
        {
            _repository = repository;
            _validator = new FlightSearchValidator();
        }

        public async Task<PagedFlightResponseDTO> Search(FlightSearchRequestDTO request)
        {

            _validator.ValidateSearch(request);

            int offset = (request.Page - 1) * request.PageSize;

            var (entities, totalCount) = await _repository.SearchFlights(
                request.Origin, request.Destination, request.Date, request.Seats, offset, request.PageSize);

            var dtos = entities.Select(e => new FlightSearchResponseDTO
            {
                Origin = e.Origin,
                Destination = e.Destination,
                Stops = e.Stops,
                LayoverAirports = e.Stops > 0 ? e.LayoverAirports : "Vuelo Directo",
                TotalTouristPrice = e.TotalTouristPrice,
                TotalFirstClassPrice = e.TotalFirstClassPrice,
                
                DepartureTimeText = e.DepartureTime.ToString("h:mm tt", new CultureInfo("es-ES")).ToLower(),
                ArrivalTimeText = e.ArrivalTime.ToString("h:mm tt", new CultureInfo("es-ES")).ToLower(),
                
                ArrivalDateText = e.ArrivalTime.Date > e.DepartureTime.Date 
                                  ? $"Llega el {e.ArrivalTime.ToString("dd MMM", new CultureInfo("es-ES"))}"
                                  : "",
                
                TotalDurationText = FormatDuration(e.TotalDurationSeconds)
            }).ToList();

            return new PagedFlightResponseDTO
            {
                TotalRecords = totalCount,
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Flights = dtos
            };
        }

        private string FormatDuration(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            int totalHours = (int)time.TotalHours;
            return $"{totalHours}H, {time.Minutes}M";
        }
    }
}