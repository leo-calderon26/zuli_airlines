using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Interface
{
    public interface IFlightResolverService
    {
        Task<List<Guid>> ResolveFlightIds(TicketPurchaseRequestDTO request);
        Task<Guid> ResolveFlightId(SummarizedFlightRoute flightRoute);
        Task<List<FlightEntity>> GetFlights(List<Guid> flightIds);
    }
}
