using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightResolverService : IFlightResolverService
    {
        private readonly IFlightRepository _flightRepository;

        public FlightResolverService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        public async Task<List<Guid>> ResolveFlightIds(TicketPurchaseRequestDTO request)
        {
            await ValidateFlightAvailability(request);

            var flightIds = new List<Guid>();

            foreach (var flightRoute in request.FlightRoutes)
            {
                var flightId = await ResolveFlightId(flightRoute);
                flightIds.Add(flightId);
            }

            var distinctFlightIds = flightIds
                .Where(flightId => flightId != Guid.Empty)
                .Distinct()
                .ToList();

            if (distinctFlightIds.Count == 0)
            {
                throw new ZuliNotFoundException("Vuelo no encontrado");
            }

            request.FlightIdList = distinctFlightIds;

            return distinctFlightIds;
        }

        public async Task<Guid> ResolveFlightId(SummarizedFlightRoute flightRoute)
        {
            var existingFlightId = await _flightRepository.GetFlightByRoute(
                flightRoute.FlightRouteId,
                flightRoute.DepartureDate
            );

            if (existingFlightId != Guid.Empty)
            {
                return existingFlightId;
            }

            return await _flightRepository.CreateFlight(
                flightRoute.FlightRouteId,
                flightRoute.DepartureDate
            );
        }

        public async Task<List<FlightEntity>> GetFlights(List<Guid> flightIds)
        {
            var flights = await _flightRepository.GetFlightsByIds(flightIds);
            var flightList = flights.ToList();
            if (flightList.Count != flightIds.Count)
            {
                throw new ZuliNotFoundException("Vuelo no encontrado");
            }
            return flightList;
        }

        public async Task ValidateFlightAvailability(TicketPurchaseRequestDTO request)
        {
            var requestedSeats = request.Passengers.Count;

            foreach (var flightRoute in request.FlightRoutes)
            {
                if (!DateTime.TryParse(flightRoute.DepartureDate, out var departureDate))
                {
                    throw new ZuliBadRequestException(
                        "flightRoutes",
                        "La fecha de salida del vuelo no tiene un formato valido."
                    );
                }

                var hasAvailability = await _flightRepository.CheckAvailability(
                    flightRoute.FlightRouteId,
                    departureDate,
                    requestedSeats
                );

                if (hasAvailability == 0)
                {
                    throw new ZuliBadRequestException(
                        "seats",
                        "No hay espacios suficientes en el vuelo seleccionado."
                    );
                }
            }
        }
    }
}
