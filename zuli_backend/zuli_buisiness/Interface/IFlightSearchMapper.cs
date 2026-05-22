using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Interface
{
    public interface IFlightSearchMapper
    {
        List<FlightSearchResponseDTO> MapToOptions(List<List<RawFlightEntity>> paths, string flightClass);
    }
}