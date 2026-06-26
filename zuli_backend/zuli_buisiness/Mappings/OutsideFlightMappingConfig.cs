using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class OutSideFlightMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<OutsideFlightDTO, OutsideFlightEntity>()
                .Map(dest => dest.AirlineId, src => src.AirlineId)
                .Map(dest => dest.TouristPrice, src => src.TouristPrice)
                .Map(dest => dest.FirstClassPrice, src => src.FirstClassPrice)
                .Map(dest => dest.CarryOnPrice, src => src.CarryOnPrice)
                .Map(dest => dest.CheckedPrice, src => src.CheckedPrice)
                .Map(dest => dest.Frequency, src => src.Frequency)
                .Map(dest => dest.DurationOnMinutes, src => src.DurationOnMinutes)
                .Map(dest => dest.ArrivalAirportCode, src => src.RealArrivalAirport.code)
                .Map(dest => dest.DepartureAirportCode, src => src.RealDepartureAirport.code)
                .Map(dest => dest.RealDepartureTime, src => src.DepartureDateTime.TimeOfDay)
                .Map(dest => dest.RealArrivalTime, src => src.ArrivalDateTime.TimeOfDay);

        }
    }
}