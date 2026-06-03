using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class FlightMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RequestedFlightDTO, RequestedFlightEntity>()
                .Map(dest => dest.destination, src => src.destination)
                .Map(dest => dest.earliestDeparture, src => src.earliestDeparture)
                .Map(dest => dest.latestDeparture, src => src.latestDeparture)
                .Map(dest => dest.passengersQuantity, src => src.passengersQuantity);
        }
    }
}